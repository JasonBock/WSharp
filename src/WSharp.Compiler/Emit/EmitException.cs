namespace WSharp.Compiler.Emit;

[Serializable]
public sealed class EmitException
	: Exception
{
	public EmitException()
		: base() { }

	public EmitException(string? message)
		: base(message) { }

	public EmitException(string? message, Exception? innerException)
		: base(message, innerException) { }
}