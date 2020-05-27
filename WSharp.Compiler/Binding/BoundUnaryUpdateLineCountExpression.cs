using System.Collections.Generic;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundUnaryUpdateLineCountExpression
		: BoundExpression
	{
		internal BoundUnaryUpdateLineCountExpression(BoundExpression lineNumber) =>
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
		public override TypeSymbol Type => this.LineNumber.Type;
	}
}