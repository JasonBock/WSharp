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
			: base() { }

		public ExecutionEngineLinesException(ImmutableList<string> messages)
			: base() => this.Messages = messages;

		public ExecutionEngineLinesException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }

		public ImmutableList<string> Messages { get; }
	}
}
