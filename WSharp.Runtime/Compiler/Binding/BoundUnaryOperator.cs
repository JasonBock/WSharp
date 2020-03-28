using System;
using System.Numerics;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler.Binding
{
	public sealed class BoundUnaryOperator
	{
		private static BoundUnaryOperator[] operators =
		{
			new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
			new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(BigInteger)),
			new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(BigInteger))
		};

		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind operatorKind, Type operandType)
			: this(syntaxKind, operatorKind, operandType, operandType) { }

		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind operatorKind, Type operandType, Type resultType) =>
			(this.SyntaxKind, this.OperatorKind, this.OperandType, this.ResultType) = (syntaxKind, operatorKind, operandType, resultType);

		public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, Type operandType)
		{
			foreach(var @operator in BoundUnaryOperator.operators)
			{
				if(@operator.SyntaxKind == syntaxKind && @operator.OperandType == operandType)
				{
					return @operator;
				}
			}

			return null;
		}

		public BoundUnaryOperatorKind OperatorKind { get; }
		public Type OperandType { get; }
		public Type ResultType { get; }
		public SyntaxKind SyntaxKind { get; }
	}
}
