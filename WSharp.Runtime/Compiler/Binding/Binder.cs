using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	public sealed class Binder
	{
		private readonly DiagnosticBag diagnostics = new DiagnosticBag();

		public DiagnosticBag Diagnostics => this.diagnostics;

		public BoundExpression BindExpression(ExpressionSyntax syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.ParenthesizedExpression:
					return this.BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
				case SyntaxKind.LiteralExpression:
					return this.BindLiteralExpression((LiteralExpressionSyntax)syntax);
				case SyntaxKind.UnaryExpression:
					return this.BindUnaryExpression((UnaryExpressionSyntax)syntax);
				case SyntaxKind.BinaryExpression:
					return this.BindBinaryExpression((BinaryExpressionSyntax)syntax);
				case SyntaxKind.UpdateLineCountExpression:
					return this.BindUpdateLineCountExpression((UpdateLineCountExpressionSyntax)syntax);
				case SyntaxKind.LineExpression:
					return this.BindLineExpression((LineExpressionSyntax)syntax);
				default:
					throw new BindingException($"Unexpected syntax {syntax.Kind}");
			}
		}

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
				this.diagnostics.ReportUndefinedLineCountOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
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
				this.diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
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
				this.diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
				return boundOperand;
			}

			return new BoundUnaryExpression(boundOperator, boundOperand);
		}

		private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
		{
			var value = syntax.Value ?? BigInteger.Zero;
			return new BoundLiteralExpression(value);
		}
	}
}