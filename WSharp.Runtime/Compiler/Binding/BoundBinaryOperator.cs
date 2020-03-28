using System;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	public sealed class BoundBinaryOperator
	{
		private static BoundBinaryOperator[] operators =
		{
			new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, typeof(BigInteger)),
			new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, typeof(BigInteger)),
			new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, typeof(BigInteger)),
			new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(BigInteger)),
			new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
			new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, typeof(bool))
		};

		private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, Type type)
			: this(syntaxKind, operatorKind, type, type, type) { }

		private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, Type leftType, Type rightType, Type returnType) =>
			(this.SyntaxKind, this.OperatorKind, this.LeftType, this.RightType, this.ReturnType) = (syntaxKind, operatorKind, leftType, rightType, returnType);

		public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
		{
			foreach(var @operator in BoundBinaryOperator.operators)
			{
				if(@operator.SyntaxKind == syntaxKind && @operator.LeftType == leftType && @operator.RightType == rightType)
				{
					return @operator;
				}
			}

			return null;
		}

		public Type LeftType { get; }
		public BoundBinaryOperatorKind OperatorKind { get; }
		public Type ReturnType { get; }
		public Type RightType { get; }
		public SyntaxKind SyntaxKind { get; }
	}
}