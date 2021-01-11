using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace WSharp.Runtime.Tests
{
	public static class ExecutionEngineLinesExceptionTests
	{
		[Test]
		public static void Create()
		{
			var exception = new ExecutionEngineLinesException();
			Assert.Multiple(() =>
			{
				Assert.That(exception.Message, Is.Not.Null, nameof(exception.Message));
				Assert.That(exception.InnerException, Is.Null, nameof(exception.InnerException));
			});
		}

		[Test]
		public static void CreateWithMessage()
		{
			const string message = "a";
			var exception = new ExecutionEngineLinesException(
				new List<string>(new[] { message }).ToImmutableArray());

			Assert.Multiple(() =>
			{
				Assert.That(exception.Messages.Length, Is.EqualTo(1), nameof(exception.Messages.Length));
				Assert.That(exception.Messages[0], Is.EqualTo(message), nameof(message));
			});
		}

		[Test]
		public static void Roundtrip()
		{
			var exception = new ExecutionEngineLinesException();
			var newException = JsonSerializer.Deserialize<ExecutionEngineLinesException>(JsonSerializer.Serialize(exception))!;

			Assert.Multiple(() =>
			{
				Assert.That(newException, Is.Not.Null);
				Assert.That(newException.Message, Is.EqualTo(exception.Message), nameof(exception.Message));
			});
		}
	}
}