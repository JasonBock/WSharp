using NUnit.Framework;
using System;
using WSharp.Tests;

namespace WSharp.Compiler.Tests
{
	public sealed class EvaluationExceptionTests
		: ExceptionTests<EvaluationException, Exception>
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