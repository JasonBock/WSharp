using System.Collections.Generic;
using System.Linq;
using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundErrorExpression
		: BoundExpression
	{
		public override IEnumerable<BoundNode> GetChildren() =>
			Enumerable.Empty<BoundNode>();

		public override IEnumerable<(string name, object value)> GetProperties() => 
			Enumerable.Empty<(string, object)>();

		public override TypeSymbol Type => TypeSymbol.Error;
		public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
	}
}