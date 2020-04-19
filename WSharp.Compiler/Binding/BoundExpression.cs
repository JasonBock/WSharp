using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public abstract class BoundExpression
		: BoundNode
	{
		public abstract TypeSymbol Type { get; }
	}
}