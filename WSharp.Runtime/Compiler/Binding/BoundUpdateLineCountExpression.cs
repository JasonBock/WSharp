using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundUpdateLineCountExpression
		: BoundExpression
	{
		public BoundUpdateLineCountExpression(BoundExpression left, BoundUpdateLineCountOperatorKind operatorKind, BoundExpression right) =>
			(this.Left, this.OperatorKind, this.Right) = (left, operatorKind, right);

		public BoundExpression Left { get; }
		public BoundUpdateLineCountOperatorKind OperatorKind { get; }
		public BoundExpression Right { get; }

		public override Type Type => this.Left.Type;

		public override BoundNodeKind Kind => BoundNodeKind.UpdateLineCountExpression;
	}
}