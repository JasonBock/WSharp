using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public abstract class BoundExpression
		: BoundNode
	{
		public virtual BoundConstant? ConstantValue { get; }
		public abstract TypeSymbol Type { get; }
	}
}