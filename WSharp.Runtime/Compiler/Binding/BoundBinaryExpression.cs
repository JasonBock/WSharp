using System;
using System.Collections.Generic;
using System.Linq;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundBinaryExpression
		: BoundExpression
	{
		public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator @operator, BoundExpression right) =>
			(this.Left, this.Operator, this.Right) = (left, @operator, right);

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.Left;
			yield return this.Right;
		}

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
		}

		public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
		public BoundExpression Left { get; }
		public BoundBinaryOperator Operator { get; }
		public BoundExpression Right { get; }
		public override Type Type => this.Operator.ResultType;
	}
}