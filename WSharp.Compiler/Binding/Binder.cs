using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	internal sealed class Binder
	{
		public BoundStatement BindCompilationUnit(CompilationUnitSyntax syntax) =>
			this.BindStatement(syntax.LineStatements);

		private BoundStatement BindStatement(StatementSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.LineStatements => this.BindLineStatements((LineStatementsSyntax)syntax),
				SyntaxKind.LineStatement => this.BindLineStatement((LineStatementSyntax)syntax),
				SyntaxKind.ExpressionStatement => this.BindExpressionStatement((ExpressionStatementSyntax)syntax),
				_ => throw new BindingException($"Unexpected statement syntax {syntax.Kind}"),
			};

		private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
		{
			var expression = this.BindExpression(syntax.Expression, true);
			return new BoundExpressionStatement(expression);
		}

		private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
		{
			var result = this.BindExpressionInternal(syntax);

			if (!canBeVoid && result.Type == TypeSymbol.Void)
			{
				this.Diagnostics.ReportExpressionMustHaveValue(syntax.Span);
				return new BoundErrorExpression();
			}

			return result;
		}
		private BoundExpression BindExpressionInternal(ExpressionSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.ParenthesizedExpression => this.BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
				SyntaxKind.LiteralExpression => this.BindLiteralExpression((LiteralExpressionSyntax)syntax),
				SyntaxKind.UnaryExpression => this.BindUnaryExpression((UnaryExpressionSyntax)syntax),
				SyntaxKind.BinaryExpression => this.BindBinaryExpression((BinaryExpressionSyntax)syntax),
				SyntaxKind.UpdateLineCountExpression => this.BindUpdateLineCountExpression((UpdateLineCountExpressionSyntax)syntax),
				SyntaxKind.UnaryUpdateLineCountExpression => this.BindUnaryUpdateLineCountExpression((UnaryUpdateLineCountExpressionSyntax)syntax),
				SyntaxKind.CallExpression => this.BindCallExpression((CallExpressionSyntax)syntax),
				_ => throw new BindingException($"Unexpected expression syntax {syntax.Kind}"),
			};

		private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
		{
			if(syntax.Arguments.Count == 1 && TypeSymbol.Lookup(syntax.Identifier.Text) is TypeSymbol type)
			{
				return this.BindConversion(syntax.Arguments[0], type);
			}

			var functions = BuiltinFunctions.GetAll();
			var function = functions.SingleOrDefault(_ => _.Name == syntax.Identifier.Text);

			if(function == null)
			{
				this.Diagnostics.ReportUndefinedFunction(syntax.Identifier);
				return new BoundErrorExpression();
			}

			if(syntax.Arguments.Count != function.Parameters.Length)
			{
				this.Diagnostics.ReportWrongArgumentCount(syntax.Span, function.Name, 
					function.Parameters.Length, syntax.Arguments.Count);
				return new BoundErrorExpression();
			}

			var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

			foreach (var argument in syntax.Arguments)
			{
				var boundArgument = this.BindExpression(argument);
				boundArguments.Add(boundArgument);
			}

			for (var i = 0; i < syntax.Arguments.Count; i++)
			{
				var argument = boundArguments[i];
				var parameter = function.Parameters[i];

				if(argument.Type != parameter.Type)
				{
					this.Diagnostics.ReportWrongArgumentType(syntax.Span, parameter.Name, 
						parameter.Type, argument.Type);
					return new BoundErrorExpression();
				}
			}

			return new BoundCallExpression(function, boundArguments.ToImmutable());
		}

		private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type)
		{
			var expression = this.BindExpression(syntax);
			var conversion = Conversion.Classify(expression.Type, type);

			if(!conversion.Exists)
			{
				if(expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
				{
					this.Diagnostics.ReportCannotConvert(syntax.Span, expression.Type, type);
				}

				return new BoundErrorExpression();
			}

			if (conversion.IsIdentity)
			{
				return expression;
			}

			return new BoundConversionExpression(expression, type);
		}

		private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) =>
			this.BindExpression(syntax.Expression);

		private BoundStatement BindLineStatements(LineStatementsSyntax syntax)
		{
			var lineStatements = new List<BoundLineStatement>();

			foreach(var line in syntax.Lines)
			{
				lineStatements.Add((BoundLineStatement)this.BindLineStatement(line));
			}

			return new BoundLineStatements(lineStatements);
		}

		private BoundStatement BindLineStatement(LineStatementSyntax syntax)
		{
			var boundLineNumber = this.BindStatement(syntax.Number);
			var boundLines = syntax.Statements.Select(_ => this.BindStatement(_)).ToList();
			return new BoundLineStatement(boundLineNumber, boundLines);
		}

		private BoundExpression BindUpdateLineCountExpression(UpdateLineCountExpressionSyntax syntax)
		{
			var boundLeft = this.BindExpression(syntax.Left);
			var boundRight = this.BindExpression(syntax.Right);

			if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression();
			}

			var boundOperatorKind = this.BindUpdateLineCountOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperatorKind == null)
			{
				this.Diagnostics.ReportUndefinedLineCountOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return new BoundErrorExpression();
			}

			return new BoundUpdateLineCountExpression(boundLeft, boundOperatorKind.Value, boundRight);
		}

		private BoundExpression BindUnaryUpdateLineCountExpression(UnaryUpdateLineCountExpressionSyntax syntax)
		{
			var boundLineNumber = this.BindExpression(syntax.LineNumber);

			if (boundLineNumber.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression();
			}

			return new BoundUnaryUpdateLineCountExpression(boundLineNumber);
		}

		private BoundUpdateLineCountOperatorKind? BindUpdateLineCountOperatorKind(SyntaxKind kind, TypeSymbol leftType, TypeSymbol rightType)
		{
			if (leftType != TypeSymbol.Integer || rightType != TypeSymbol.Integer)
			{
				return null;
			}

			if (kind == SyntaxKind.UpdateLineCountToken)
			{
				return BoundUpdateLineCountOperatorKind.Update;
			}
			else
			{
				throw new BindingException($"Unexpected update line count operator {kind}");
			}
		}

		private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
		{
			var boundLeft = this.BindExpression(syntax.Left);
			var boundRight = this.BindExpression(syntax.Right);

			if(boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression();
			}

			var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return new BoundErrorExpression();
			}

			return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
		}

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var boundOperand = this.BindExpression(syntax.Operand);

			if (boundOperand.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression();
			}

			var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
				return new BoundErrorExpression();
			}

			return new BoundUnaryExpression(boundOperator, boundOperand);
		}

		private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
		{
			var value = syntax.Value ?? BigInteger.Zero;
			return new BoundLiteralExpression(value);
		}

		public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();
	}
}