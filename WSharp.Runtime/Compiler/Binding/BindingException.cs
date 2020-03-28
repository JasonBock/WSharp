using System;
using System.Runtime.Serialization;

namespace WSharp.Runtime.Compiler.Binding
{
	[Serializable]
	internal class BindingException : Exception
	{
		public BindingException()
		{
		}

		public BindingException(string? message) : base(message)
		{
		}

		public BindingException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected BindingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}