using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundUnaryExpression
		: BoundExpression
	{
		public BoundUnaryExpression(BoundUnaryOperator @operator, BoundExpression operand) => 
			(this.Operator, this.Operand) = (@operator, operand);

		public BoundUnaryOperator Operator { get; }
		public BoundExpression Operand { get; }

		public override Type Type => this.Operand.Type;

		public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
	}
}