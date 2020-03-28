using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundBinaryExpression
		: BoundExpression
	{
		public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind operatorKind, BoundExpression right) =>
			(this.Left, this.OperatorKind, this.Right) = (left, operatorKind, right);

		public BoundExpression Left { get; }
		public BoundBinaryOperatorKind OperatorKind { get; }
		public BoundExpression Right { get; }

		public override Type Type => this.Left.Type;

		public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
	}
}