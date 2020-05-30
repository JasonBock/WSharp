using NUnit.Framework;
using System;
using WSharp.Compiler.Binding;
using WSharp.Tests;

namespace WSharp.Compiler.Tests.Binding
{
	public sealed class BindingExceptionTests
		: ExceptionTests<BindingException, Exception>
	{
		[Test]
		public void Create() => base.CreateExceptionTest();

		[Test]
		public void CreateWithMessage() =>
			base.CreateExceptionWithMessageTest(Guid.NewGuid().ToString("N"));

		[Test]
		public void CreateWithMessageAndInnerException() =>
			base.CreateExceptionWithMessageAndInnerExceptionTest(Guid.NewGuid().ToString("N"));

		[Test]
		public void Roundtrip() =>
			base.RoundtripExceptionTest(Guid.NewGuid().ToString("N"));
	}
}