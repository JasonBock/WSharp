using System.Runtime.Serialization;

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

	private EmitException(SerializationInfo info, StreamingContext context)
		: base(info, context) { }
}