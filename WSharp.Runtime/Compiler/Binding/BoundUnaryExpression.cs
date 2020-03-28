using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundUnaryExpression
		: BoundExpression
	{
		public BoundUnaryExpression(BoundUnaryOperatorKind operatorKind, BoundExpression operand) => 
			(this.OperatorKind, this.Operand) = (operatorKind, operand);

		public BoundUnaryOperatorKind OperatorKind { get; }
		public BoundExpression Operand { get; }

		public override Type Type => this.Operand.Type;

		public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
	}
}