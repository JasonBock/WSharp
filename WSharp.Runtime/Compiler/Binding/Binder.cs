using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	// TODO: Consider making this internal
	public sealed class Binder
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
			var expression = this.BindExpression(syntax.Expression);
			return new BoundExpressionStatement(expression);
		}

		private BoundExpression BindExpression(ExpressionSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.ParenthesizedExpression => this.BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
				SyntaxKind.NameExpression => this.BindNameExpression((NameExpressionSyntax)syntax),
				SyntaxKind.LiteralExpression => this.BindLiteralExpression((LiteralExpressionSyntax)syntax),
				SyntaxKind.UnaryExpression => this.BindUnaryExpression((UnaryExpressionSyntax)syntax),
				SyntaxKind.BinaryExpression => this.BindBinaryExpression((BinaryExpressionSyntax)syntax),
				SyntaxKind.UpdateLineCountExpression => this.BindUpdateLineCountExpression((UpdateLineCountExpressionSyntax)syntax),
				_ => throw new BindingException($"Unexpected expression syntax {syntax.Kind}"),
			};

		private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
		{
			var name = syntax.IdentifierToken.Text;

			if(string.IsNullOrWhiteSpace(name))
			{
				return new BoundLiteralExpression(0);
			}
			else
			{
				// TODO: This should be a method invocation on the context. For now, do this:
				return new BoundLiteralExpression(0);
			}
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
			var boundOperatorKind = this.BindUpdateLineCountOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperatorKind == null)
			{
				this.Diagnostics.ReportUndefinedLineCountOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return boundLeft;
			}

			return new BoundUpdateLineCountExpression(boundLeft, boundOperatorKind.Value, boundRight);
		}

		private BoundUpdateLineCountOperatorKind? BindUpdateLineCountOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
		{
			if (leftType != typeof(BigInteger) || rightType != typeof(BigInteger))
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
			var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return boundLeft;
			}

			return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
		}

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var boundOperand = this.BindExpression(syntax.Operand);
			var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

			if (boundOperator == null)
			{
				this.Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
				return boundOperand;
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