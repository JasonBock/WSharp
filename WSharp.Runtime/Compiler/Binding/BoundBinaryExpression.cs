using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundBinaryExpression
		: BoundExpression
	{
		public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator @operator, BoundExpression right) =>
			(this.Left, this.Operator, this.Right) = (left, @operator, right);

		public BoundExpression Left { get; }
		public BoundBinaryOperator Operator { get; }
		public BoundExpression Right { get; }

		public override Type Type => this.Left.Type;

		public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
	}
}