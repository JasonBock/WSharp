using NUnit.Framework;
using System.Text.Json;

namespace WSharp.Runtime.Tests;

internal static class ExecutionEngineLinesExceptionTests
{
	[Test]
	public static void Create()
	{
		var exception = new ExecutionEngineLinesException();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.Not.Null, nameof(exception.Message));
			Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
		}
	}

	[Test]
	public static void CreateWithMessage()
	{
		const string message = "a";
		var exception = new ExecutionEngineLinesException([message]);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Messages, Has.Length.EqualTo(1), nameof(exception.Messages.Length));
			Assert.That(exception.Messages[0], Is.EqualTo(message), nameof(message));
		}
	}

	[Test]
	public static void Roundtrip()
	{
		var exception = new ExecutionEngineLinesException();
		var newException = JsonSerializer.Deserialize<ExecutionEngineLinesException>(JsonSerializer.Serialize(exception))!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newException, Is.Not.Null);
			Assert.That(newException.Message, Is.EqualTo(exception.Message), nameof(exception.Message));
		}
	}
}