using System.Collections.Generic;
using System.Linq;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundErrorExpression
		: BoundExpression
	{
		public override IEnumerable<BoundNode> GetChildren() =>
			Enumerable.Empty<BoundNode>();

		public override IEnumerable<(string name, object value)> GetProperties() => 
			Enumerable.Empty<(string, object)>();

		public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
		public override TypeSymbol Type => TypeSymbol.Error;
	}
}