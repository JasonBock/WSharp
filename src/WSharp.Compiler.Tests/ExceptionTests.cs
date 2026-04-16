using NUnit.Framework;

namespace WSharp.Compiler.Tests;

internal abstract class ExceptionTests<T, TInner>
	where T : Exception, new()
	where TInner : Exception, new()
{
	protected ExceptionTests()
		: base()
	{ }

#pragma warning disable CA1822 // Mark members as static
	protected void CreateExceptionTest()
	{
		var exception = new T();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.Not.Null, nameof(exception.Message));
			Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
		}
	}

	protected void CreateExceptionWithMessageTest(string message)
	{
		var exception = (T)Activator.CreateInstance(typeof(T), message)!;
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo(message), nameof(exception.Message));
			Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
		}
	}

	protected void CreateExceptionWithMessageAndInnerExceptionTest(string message)
	{
		var innerException = new TInner();
		var exception = (T)Activator.CreateInstance(typeof(T), message, innerException)!;
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo(message), nameof(exception.Message));
			Assert.That(exception.InnerException, Is.EqualTo(innerException), nameof(exception.InnerException));
		}
	}
#pragma warning restore CA1822 // Mark members as static
}