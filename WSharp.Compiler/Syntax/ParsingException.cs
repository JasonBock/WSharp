using System;
using System.Runtime.Serialization;

namespace WSharp.Compiler.Syntax
{
	[Serializable]
	public sealed class ParsingException
		: Exception
	{
		public ParsingException() { }

		public ParsingException(string? message)
			: base(message) { }

		public ParsingException(string? message, Exception? innerException)
			: base(message, innerException) { }

		private ParsingException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}