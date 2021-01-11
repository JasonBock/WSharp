using NUnit.Framework;
using System;
using System.Text.Json;

namespace WSharp.Tests
{
	public abstract class ExceptionTests<T, TInner>
		where T : Exception, new()
		where TInner : Exception, new()
	{
		protected ExceptionTests()
			: base()
		{ }

		protected void CreateExceptionTest()
		{
			var exception = new T();
			Assert.Multiple(() =>
			{
				Assert.That(exception.Message, Is.Not.Null, nameof(exception.Message));
				Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
			});
		}

		protected void CreateExceptionWithMessageTest(string message)
		{
			var exception = (T)Activator.CreateInstance(typeof(T), message)!;
			Assert.Multiple(() =>
			{
				Assert.That(exception.Message, Is.EqualTo(message), nameof(exception.Message));
				Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
			});
		}

		protected void CreateExceptionWithMessageAndInnerExceptionTest(string message)
		{
			var innerException = new TInner();
			var exception = (T)Activator.CreateInstance(typeof(T), message, innerException)!;
			Assert.Multiple(() =>
			{
				Assert.That(exception.Message, Is.EqualTo(message), nameof(exception.Message));
				Assert.That(exception.InnerException, Is.EqualTo(innerException), nameof(exception.InnerException));
			});
		}
	}
}
