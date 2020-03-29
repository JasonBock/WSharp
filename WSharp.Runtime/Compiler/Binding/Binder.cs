using System;
using System.Linq;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	// TODO: Consider making this internal
	public sealed class Binder
	{
		public BoundExpression BindExpression(ExpressionSyntax syntax) =>
			syntax.Kind switch
			{
				SyntaxKind.ParenthesizedExpression => this.BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
				SyntaxKind.LiteralExpression => this.BindLiteralExpression((LiteralExpressionSyntax)syntax),
				SyntaxKind.UnaryExpression => this.BindUnaryExpression((UnaryExpressionSyntax)syntax),
				SyntaxKind.BinaryExpression => this.BindBinaryExpression((BinaryExpressionSyntax)syntax),
				SyntaxKind.UpdateLineCountExpression => this.BindUpdateLineCountExpression((UpdateLineCountExpressionSyntax)syntax),
				SyntaxKind.LineExpression => this.BindLineExpression((LineExpressionSyntax)syntax),
				_ => throw new BindingException($"Unexpected syntax {syntax.Kind}"),
			};

		private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax) =>
			this.BindExpression(syntax.Expression);

		private BoundExpression BindLineExpression(LineExpressionSyntax syntax)
		{
			var boundLineNumber = this.BindExpression(syntax.Number);
			var boundLines = syntax.Expressions.Select(_ => this.BindExpression(_)).ToList();
			return new BoundLineExpression(boundLineNumber, boundLines);
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