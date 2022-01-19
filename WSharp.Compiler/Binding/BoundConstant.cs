namespace WSharp.Compiler.Binding;

public sealed class BoundConstant
{
	public BoundConstant(object value) =>
		this.Value = value;

	public object Value { get; }
}