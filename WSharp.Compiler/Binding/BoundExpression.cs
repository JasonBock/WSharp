using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public abstract class BoundExpression
		: BoundNode
	{
		public virtual BoundConstant? ConstantValue { get; }
		public abstract TypeSymbol Type { get; }
	}

	public sealed class BoundConstant
	{
		public BoundConstant(object value) =>
			this.Value = value;

		public object Value { get; }
	}
}