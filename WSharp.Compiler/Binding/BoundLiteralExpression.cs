using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundLiteralExpression
		: BoundExpression
	{
		internal BoundLiteralExpression(object value)
		{
			this.Type = value is bool ? TypeSymbol.Boolean :
				value is BigInteger ? TypeSymbol.Integer :
				value is string ? TypeSymbol.String : throw new BindingException($"Unexpected literal '{value}' of type {value.GetType()}.");
			this.ConstantValue = new BoundConstant(value);
 		}

		public override IEnumerable<BoundNode> GetChildren() => 
			Enumerable.Empty<BoundNode>();

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
			yield return (nameof(this.Value), this.Value);
		}

		public override BoundConstant ConstantValue { get; }
		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
		public override TypeSymbol Type { get; }
		public object Value => this.ConstantValue!.Value;
	}
}