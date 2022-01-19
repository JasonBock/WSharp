using System.Runtime.Serialization;

namespace WSharp.Runtime;

[Serializable]
public sealed class EngineException
	: Exception
{
	public EngineException()
		: base() { }

	public EngineException(string? message)
		: base(message) { }

	public EngineException(string? message, Exception? innerException)
		: base(message, innerException) { }

	private EngineException(SerializationInfo info, StreamingContext context)
		: base(info, context) { }
}