using System.Collections.Generic;
using System.Linq;
using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundConversionExpression
		: BoundExpression
	{
		public BoundConversionExpression(TypeSymbol type, BoundExpression expression) =>
			(this.Type, this.Expression) = (type, expression);


		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.Expression;
		}

		public override IEnumerable<(string name, object value)> GetProperties() => 
			Enumerable.Empty<(string, object)>();

		public BoundExpression Expression { get; }
		public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
		public override TypeSymbol Type { get; }
	}
}