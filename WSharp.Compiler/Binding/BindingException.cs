using System;
using System.Runtime.Serialization;

namespace WSharp.Compiler.Binding
{
	[Serializable]
	public sealed class BindingException 
		: Exception
	{
		public BindingException()
			: base() { }

		public BindingException(string? message)
			: base(message) { }

		public BindingException(string? message, Exception? innerException)
			: base(message, innerException) { }

		private BindingException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}