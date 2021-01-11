using NUnit.Framework;
using System;
using WSharp.Compiler.Syntax;
using WSharp.Tests;

namespace WSharp.Compiler.Tests.Syntax
{
	public sealed class ParsingExceptionTests
		: ExceptionTests<ParsingException, Exception>
	{
		[Test]
		public void Create() => base.CreateExceptionTest();

		[Test]
		public void CreateWithMessage() =>
			base.CreateExceptionWithMessageTest(Guid.NewGuid().ToString("N"));

		[Test]
		public void CreateWithMessageAndInnerException() =>
			base.CreateExceptionWithMessageAndInnerExceptionTest(Guid.NewGuid().ToString("N"));
	}
}