using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundLiteralExpression
		: BoundExpression
	{
		public BoundLiteralExpression(object value) => this.Value = value;

		public object Value { get; }

		public override Type Type => this.Value.GetType();

		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
	}
}