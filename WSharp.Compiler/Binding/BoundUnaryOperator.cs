using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundUnaryOperator
	{
		private static readonly BoundUnaryOperator[] operators =
		{
			new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Boolean),
			new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Integer),
			new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Integer),
			new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.Integer),
		};

		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind operatorKind, TypeSymbol operandType)
			: this(syntaxKind, operatorKind, operandType, operandType) { }

		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind operatorKind, TypeSymbol operandType, TypeSymbol resultType) =>
			(this.SyntaxKind, this.OperatorKind, this.OperandType, this.ResultType) = (syntaxKind, operatorKind, operandType, resultType);

		public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
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
		public TypeSymbol OperandType { get; }
		public TypeSymbol ResultType { get; }
		public SyntaxKind SyntaxKind { get; }
	}
}
