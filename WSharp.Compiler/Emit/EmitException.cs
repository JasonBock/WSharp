using System;
using System.Runtime.Serialization;

namespace WSharp.Compiler.Emit
{
	[Serializable]
	internal class EmitException : Exception
	{
		public EmitException()
		{
		}

		public EmitException(string? message) : base(message)
		{
		}

		public EmitException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected EmitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}