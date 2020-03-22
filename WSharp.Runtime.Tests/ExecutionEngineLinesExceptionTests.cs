using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
				new List<string>(new[] { message }).ToImmutableList());

			Assert.Multiple(() =>
			{
				Assert.That(exception.Messages.Count, Is.EqualTo(1), nameof(exception.Messages.Count));
				Assert.That(exception.Messages[0], Is.EqualTo(message), nameof(message));
			});
		}

		[Test]
		public static void Roundtrip()
		{
			var exception = new ExecutionEngineLinesException();
			var formatter = new BinaryFormatter();

			using var stream = new MemoryStream();
			formatter.Serialize(stream, exception);
			stream.Position = 0;
			var newException = (ExecutionEngineLinesException)formatter.Deserialize(stream);

			Assert.Multiple(() =>
			{
				Assert.That(newException, Is.Not.Null);
				Assert.That(newException.Message, Is.EqualTo(exception.Message), nameof(exception.Message));
			});
		}
	}
}