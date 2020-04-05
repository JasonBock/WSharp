using System;
using System.Collections.Generic;
using System.Linq;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundLiteralExpression
		: BoundExpression
	{
		public BoundLiteralExpression(object value) => this.Value = value;

		public override IEnumerable<BoundNode> GetChildren() => 
			Enumerable.Empty<BoundNode>();

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
			yield return (nameof(this.Value), this.Value);
		}

		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
		public override Type Type => this.Value.GetType();
		public object Value { get; }
	}
}