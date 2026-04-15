using System.Collections.Immutable;

namespace WSharp.Runtime;

[Serializable]
public sealed class ExecutionEngineLinesException
	: Exception
{
	public ExecutionEngineLinesException()
		: base() => this.Messages = ImmutableArray<string>.Empty;

	public ExecutionEngineLinesException(string message)
		: base(message) { }

	public ExecutionEngineLinesException(string message, Exception innerException)
		: base(message, innerException) { }

	public ExecutionEngineLinesException(ImmutableArray<string> messages)
		: base() => this.Messages = messages;

	public ImmutableArray<string> Messages { get; }
}