using System;
using System.Collections.Generic;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class Binder
	{
		private readonly List<string> diagnostics = new List<string>();

		public IEnumerable<string> Diagnostics => this.diagnostics;

		public BoundExpression BindExpression(ExpressionSyntax syntax)
		{
			switch(syntax.Kind)
			{
				case SyntaxKind.LiteralExpression:
					return this.BindLiteralExpression((LiteralExpressionSyntax)syntax);
				case SyntaxKind.UnaryExpression:
					return this.BindUnaryExpression((UnaryExpressionSyntax)syntax);
				case SyntaxKind.BinaryExpression:
					return this.BindBinaryExpression((BinaryExpressionSyntax)syntax);
				default:
					throw new BindingException($"Unexpected syntax {syntax.Kind}");
			}
		}

		private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
		{
			var boundLeft = this.BindExpression(syntax.Left);
			var boundRight = this.BindExpression(syntax.Right);
			var boundOperatorKind = this.BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundOperatorKind == null)
			{
				this.diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeft.Type} and {boundRight.Type}.");
				return boundLeft;
			}

			return new BoundBinaryExpression(boundLeft, boundOperatorKind.Value, boundRight);
		}

		private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
		{
			if(leftType != typeof(BigInteger) || rightType != typeof(BigInteger))
			{
				return null;
			}

			switch (kind)
			{
				case SyntaxKind.PlusToken:
					return BoundBinaryOperatorKind.Addition;
				case SyntaxKind.MinusToken:
					return BoundBinaryOperatorKind.Subtraction;
				case SyntaxKind.StarToken:
					return BoundBinaryOperatorKind.Multiplication;
				case SyntaxKind.SlashToken:
					return BoundBinaryOperatorKind.Division;
				default:
					throw new BindingException($"Unexpected binary operator {kind}");
			}
		}

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var boundOperand = this.BindExpression(syntax.Operand);
			var boundOperatorKind = this.BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);

			if(boundOperatorKind == null)
			{
				this.diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}.");
				return boundOperand;
			}

			return new BoundUnaryExpression(boundOperatorKind.Value, boundOperand);
		}

		private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
		{
			if(operandType != typeof(BigInteger))
			{
				return null;
			}

			switch(kind)
			{
				case SyntaxKind.PlusToken:
					return BoundUnaryOperatorKind.Identity;
				case SyntaxKind.MinusToken:
					return BoundUnaryOperatorKind.Negation;
				default:
					throw new BindingException($"Unexpected unary operator {kind}");
			}
		}

		private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
		{
			var value = syntax.LiteralToken.Value as BigInteger? ?? BigInteger.Zero;
			return new BoundLiteralExpression(value);
		}
	}
}