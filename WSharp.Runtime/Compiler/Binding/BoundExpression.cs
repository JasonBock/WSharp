using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	public abstract class BoundExpression
		: BoundNode
	{
		public abstract TypeSymbol Type { get; }
	}
}