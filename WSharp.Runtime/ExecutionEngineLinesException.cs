using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace WSharp.Runtime
{
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

		private ExecutionEngineLinesException(SerializationInfo info, StreamingContext context)
			: base(info, context) => this.Messages = ImmutableArray<string>.Empty;

		public ImmutableArray<string> Messages { get; }
	}
}