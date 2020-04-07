using System;
using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundUnaryUpdateLineCountExpression
		: BoundExpression
	{
		public BoundUnaryUpdateLineCountExpression(BoundExpression lineNumber) =>
			this.LineNumber = lineNumber;

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.LineNumber;
		}

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
		}

		public override BoundNodeKind Kind => BoundNodeKind.UnaryUpdateLineCountExpression;
		public BoundExpression LineNumber { get; }
		public override Type Type => this.LineNumber.Type;
	}
}