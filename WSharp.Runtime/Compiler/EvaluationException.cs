using System;
using System.Runtime.Serialization;

namespace WSharp.Runtime.Compiler
{
	[Serializable]
	public sealed class EvaluationException
		: Exception
	{
		public EvaluationException() { }

		public EvaluationException(string? message)
			: base(message) { }

		public EvaluationException(string? message, Exception? innerException)
			: base(message, innerException) { }

		private EvaluationException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}