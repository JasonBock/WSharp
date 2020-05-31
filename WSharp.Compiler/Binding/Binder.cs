using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Binding
{
	public sealed class Binder
	{
		private CallExpressionSyntax? deferWasInvoked;
		private bool doesStatementExistAfterDefer;

		internal Binder(CompilationUnitSyntax syntax)
		{
			var unit = this.BindStatement(syntax.LineStatements);

			foreach (var (targetLineNumber, location) in this.LineNumberValidations)
			{
				if (!this.LineNumbers.Contains(targetLineNumber))
				{
					this.Diagnostics.ReportInvalidLineNumberReference(location, targetLineNumber);
				}
			}

			this.CompilationUnit = unit;
		}

		private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
		{
			var boundLeft = this.BindExpression(syntax.Left);
			var boundRight = this.BindExpression(syntax.Right);

			if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression(syntax);
			}

			var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return new BoundErrorExpression(syntax);
			}

			return new BoundBinaryExpression(syntax, boundLeft, boundOperator, boundRight);
		}

		private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
		{
			if (syntax.Arguments.Count == 1 && TypeSymbol.Lookup(syntax.Identifier.Text) is TypeSymbol type)
			{
				return this.BindConversion(syntax.Arguments[0], type, allowExplicit: true);
			}

			var functions = BuiltinFunctions.GetAll();
			var function = functions.SingleOrDefault(_ => _.Name == syntax.Identifier.Text);

			if (function == null)
			{
				this.Diagnostics.ReportUndefinedFunction(syntax.Identifier);
				return new BoundErrorExpression(syntax);
			}

			if (syntax.Arguments.Count != function.Parameters.Length)
			{
				this.Diagnostics.ReportWrongArgumentCount(syntax.Location, function.Name,
					function.Parameters.Length, syntax.Arguments.Count);
				return new BoundErrorExpression(syntax);
			}

			var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

			foreach (var argument in syntax.Arguments)
			{
				var boundArgument = this.BindExpression(argument);
				boundArguments.Add(boundArgument);
			}

			for (var i = 0; i < syntax.Arguments.Count; i++)
			{
				var argumentLocation = syntax.Arguments[i].Location;
				var argument = boundArguments[i];
				var parameter = function.Parameters[i];
				boundArguments[i] = this.BindConversion(argumentLocation, argument, parameter.Type);
			}

			if (function == BuiltinFunctions.E || function == BuiltinFunctions.N)
			{
				if (boundArguments[0] is BoundLiteralExpression literalArgument &&
					literalArgument.Type == TypeSymbol.Integer)
				{
					var targetLineNumber = (BigInteger)literalArgument.Value;
					this.LineNumberValidations.Add((targetLineNumber, syntax.Arguments[0].Location));
				}
			}

			if (function == BuiltinFunctions.Defer)
			{
				this.deferWasInvoked = syntax;
				this.doesStatementExistAfterDefer = false;
			}

			return new BoundCallExpression(syntax, function, boundArguments.ToImmutable());
		}

		private BoundExpression BindConversion(ExpressionSyntax syntax, TypeSymbol type, bool allowExplicit = false) =>
			this.BindConversion(syntax.Location, this.BindExpression(syntax), type, allowExplicit);

		private BoundExpression BindConversion(TextLocation location, BoundExpression expression, TypeSymbol type, bool allowExplicit = false)
		{
			var conversion = Conversion.Classify(expression.Type, type);

			if (!conversion.Exists)
			{
				if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
				{
					this.Diagnostics.ReportCannotConvert(location, expression.Type, type);
				}

				return new BoundErrorExpression(expression.Syntax);
			}

			if (!allowExplicit && conversion.IsExplicit)
			{
				this.Diagnostics.ReportCannotConvertImplicitly(location, expression.Type, type);
			}

			if (conversion.IsIdentity)
			{
				return expression;
			}

			return new BoundConversionExpression(expression.Syntax, expression, type);
		}

		private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
		{
			var result = this.BindExpressionInternal(syntax);

			if (!canBeVoid && result.Type == TypeSymbol.Void)
			{
				this.Diagnostics.ReportExpressionMustHaveValue(syntax.Location);
				return new BoundErrorExpression(syntax);
			}

			return result;
		}

		private BoundExpression BindExpressionInternal(ExpressionSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.ParenthesizedExpression => this.BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
				SyntaxKind.LiteralExpression => Binder.BindLiteralExpression((LiteralExpressionSyntax)syntax),
				SyntaxKind.UnaryExpression => this.BindUnaryExpression((UnaryExpressionSyntax)syntax),
				SyntaxKind.BinaryExpression => this.BindBinaryExpression((BinaryExpressionSyntax)syntax),
				SyntaxKind.UpdateLineCountExpression => this.BindUpdateLineCountExpression((UpdateLineCountExpressionSyntax)syntax),
				SyntaxKind.UnaryUpdateLineCountExpression => this.BindUnaryUpdateLineCountExpression((UnaryUpdateLineCountExpressionSyntax)syntax),
				SyntaxKind.CallExpression => this.BindCallExpression((CallExpressionSyntax)syntax),
				_ => throw new BindingException($"Unexpected expression syntax {syntax.Kind}"),
			};

		private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax) =>
			new BoundExpressionStatement(syntax, this.BindExpression(syntax.Expression, true));

		private BoundStatement BindLineStatements(LineStatementsSyntax syntax)
		{
			var lineStatements = new List<BoundLineStatement>();

			foreach (var line in syntax.Lines)
			{
				lineStatements.Add((BoundLineStatement)this.BindLineStatement(line));
			}

			return new BoundLineStatements(syntax, lineStatements);
		}

		private BoundStatement BindLineStatement(LineStatementSyntax syntax)
		{
			var lineNumber = (BigInteger)((LiteralExpressionSyntax)syntax.Number.Expression).Value!;

			if(this.LineNumbers.Contains(lineNumber))
			{
				this.Diagnostics.ReportDuplicateLineNumber(syntax.Number.Location, lineNumber);
			}
			else
			{
				this.LineNumbers.Add(lineNumber);
			}

			this.deferWasInvoked = null;
			this.doesStatementExistAfterDefer = false;

			var boundLineNumber = this.BindStatement(syntax.Number);
			var boundLines = new List<BoundStatement>();

			foreach (var lineStatement in syntax.Statements)
			{
				this.doesStatementExistAfterDefer = this.deferWasInvoked is { };
				boundLines.Add(this.BindStatement(lineStatement));
			}

			if (this.deferWasInvoked is { } && !this.doesStatementExistAfterDefer)
			{
				this.Diagnostics.ReportNoStatementsAfterDefer(this.deferWasInvoked.Location);
			}

			return new BoundLineStatement(syntax, boundLineNumber, boundLines);
		}

		private static BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax) =>
			new BoundLiteralExpression(syntax, syntax.Value ?? BigInteger.Zero);

		private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) =>
			this.BindExpression(syntax.Expression);

		private BoundStatement BindStatement(StatementSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.LineStatements => this.BindLineStatements((LineStatementsSyntax)syntax),
				SyntaxKind.LineStatement => this.BindLineStatement((LineStatementSyntax)syntax),
				SyntaxKind.ExpressionStatement => this.BindExpressionStatement((ExpressionStatementSyntax)syntax),
				_ => throw new BindingException($"Unexpected statement syntax {syntax.Kind}"),
			};

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var boundOperand = this.BindExpression(syntax.Operand);

			if (boundOperand.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression(syntax);
			}

			var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundOperand.Type);
				return new BoundErrorExpression(syntax);
			}

			return new BoundUnaryExpression(syntax, boundOperator, boundOperand);
		}

		private BoundExpression BindUnaryUpdateLineCountExpression(UnaryUpdateLineCountExpressionSyntax syntax)
		{
			var boundLineNumber = this.BindExpression(syntax.LineNumber);

			if (boundLineNumber.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression(syntax);
			}

			if (boundLineNumber is BoundLiteralExpression literalArgument &&
				literalArgument.Type == TypeSymbol.Integer)
			{
				var targetLineNumber = (BigInteger)literalArgument.Value;
				this.LineNumberValidations.Add((targetLineNumber, syntax.Location));
			}

			return new BoundUnaryUpdateLineCountExpression(syntax, boundLineNumber);
		}

		private BoundExpression BindUpdateLineCountExpression(UpdateLineCountExpressionSyntax syntax)
		{
			var boundLeft = this.BindExpression(syntax.Left);
			var boundRight = this.BindExpression(syntax.Right);

			if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
			{
				return new BoundErrorExpression(syntax);
			}

			var boundOperatorKind = Binder.BindUpdateLineCountOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperatorKind == null)
			{
				this.Diagnostics.ReportUndefinedLineCountOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return new BoundErrorExpression(syntax);
			}

			if (boundLeft is BoundLiteralExpression literalArgument &&
				literalArgument.Type == TypeSymbol.Integer)
			{
				var targetLineNumber = (BigInteger)literalArgument.Value;
				this.LineNumberValidations.Add((targetLineNumber, syntax.Left.Location));
			}

			return new BoundUpdateLineCountExpression(syntax, boundLeft, boundOperatorKind.Value, boundRight);
		}

		private static BoundUpdateLineCountOperatorKind? BindUpdateLineCountOperatorKind(SyntaxKind kind, TypeSymbol leftType, TypeSymbol rightType)
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

		internal BoundStatement CompilationUnit { get; }
		public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();
		public HashSet<BigInteger> LineNumbers { get; } = new HashSet<BigInteger>();
		public List<(BigInteger, TextLocation)> LineNumberValidations { get; } = new List<(BigInteger, TextLocation)>();
	}
}