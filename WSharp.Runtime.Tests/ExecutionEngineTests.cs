using NUnit.Framework;
using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;

namespace WSharp.Runtime.Tests
{
	public static class ExecutionEngineTests
	{
		[Test]
		public static void Create()
		{
			var lines = ImmutableArray.Create(new Line(1, 1, _ => { }));
			using var random = new SecureRandom();
			using var writer = new StringWriter();
			var reader = new StringReader(string.Empty);

			Assert.That(() => new ExecutionEngine(lines, random, reader, writer), Throws.Nothing);
		}

		[Test]
		public static void CreateWhenGivenNullRandom() =>
			Assert.That(() => new ExecutionEngine(ImmutableArray.Create(new Line(1, 1, _ => { })), null!, new StringReader(string.Empty), new StringWriter()), 
				Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenGivenNullWriter() =>
			Assert.That(() => new ExecutionEngine(ImmutableArray.Create(new Line(1, 1, _ => { })), new SecureRandom(), new StringReader(string.Empty), null!), 
				Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenGivenNullReader() =>
			Assert.That(() => new ExecutionEngine(ImmutableArray.Create(new Line(1, 1, _ => { })), new SecureRandom(), null!, new StringWriter()),
				Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenGivenEmptyList()
		{
			var lines = ImmutableArray<Line>.Empty;
			Assert.That(() => new ExecutionEngine(lines, new SecureRandom(), new StringReader(string.Empty), new StringWriter()), 
				Throws.TypeOf<ArgumentException>()
					.And.Message.EqualTo("Must pass in at least one line. (Parameter 'lines')"));
		}

		[Test]
		public static void CreateWhenListContainsNullEntries()
		{
			var lines = ImmutableArray.Create(new Line(1, 1, _ => { }), null!, new Line(2, 1, _ => { }), null!);
			Assert.That(() => new ExecutionEngine(lines, new SecureRandom(), new StringReader(string.Empty), new StringWriter()), 
				Throws.TypeOf<ExecutionEngineLinesException>()
					.With.Property(nameof(ExecutionEngineLinesException.Messages)).Contains("The line at index 1 is null."));
		}

		[Test]
		public static void CreateWhenListContainsDuplicateIdentifiers()
		{
			var lines = ImmutableArray.Create(new Line(1, 1, _ => { }), new Line(1, 1, _ => { }));
			Assert.That(() => new ExecutionEngine(lines, new SecureRandom(), new StringReader(string.Empty), new StringWriter()), Throws.TypeOf<ArgumentException>());
		}

		[TestCase(1ul, 1ul, 1ul, true)]
		[TestCase(1ul, 0ul, 1ul, false)]
		public static void CallDoesLineExist(ulong identifier, ulong count, ulong identifierToSearch, bool expectedDoesLineExist)
		{
			var lines = ImmutableArray.Create(new Line(identifier, count, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(engine.E(identifierToSearch), Is.EqualTo(expectedDoesLineExist));
		}

		[Test]
		public static void CallDoesLineExistWhereLineDoesNotExist()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(() => engine.E(2), Throws.TypeOf<KeyNotFoundException>());
		}

		[Test]
		public static void CallExecute()
		{
			var codeExecutionCount = 0;
			var lines = ImmutableArray.Create(new Line(1, 1, _ => { codeExecutionCount++; }));

			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());
			engine.Execute();
			Assert.That(codeExecutionCount, Is.EqualTo(1));
		}

		[Test]
		public static void CallExecuteWithMultipleLines()
		{
			var codeExecutionCount0 = BigInteger.Zero;
			var line0 = new Line(1, 2, _ => { codeExecutionCount0++; });
			var codeExecutionCount1 = BigInteger.Zero;
			var line1 = new Line(2, 3, _ => { codeExecutionCount1++; });
			var codeExecutionCount2 = BigInteger.Zero;
			var line2 = new Line(3, 4, _ => { codeExecutionCount2++; });

			var lines = ImmutableArray.Create(line0, line1, line2);

			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());
			engine.Execute();

			Assert.Multiple(() =>
			{
				Assert.That(codeExecutionCount0, Is.EqualTo(line0.Count));
				Assert.That(codeExecutionCount1, Is.EqualTo(line1.Count));
				Assert.That(codeExecutionCount2, Is.EqualTo(line2.Count));
			});
		}

		[Test]
		public static void CallGetCurrentLineCount()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }), new Line(2, 4, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(engine.GetCurrentLineCount(), Is.EqualTo(new BigInteger(7)));
		}

		[Test]
		public static void CallN()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(engine.N(1), Is.EqualTo(new BigInteger(3)));
		}

		[Test]
		public static void CallNWhereLineDoesNotExist()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(() => engine.N(2), Throws.TypeOf<KeyNotFoundException>());
		}

		[Test]
		public static void CallPrint()
		{
			const string message = "hello";
			var writer = new StringWriter();
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), writer);
			engine.Print(message);

			Assert.That(writer.GetStringBuilder().ToString(), Is.EqualTo($"{message}{Environment.NewLine}"));
		}

		[TestCase("hello")]
		[TestCase("")]
		public static void CallRead(string message)
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(message), new StringWriter());

			Assert.That(engine.Read(), Is.EqualTo(message));
		}

		[TestCase(true)]
		[TestCase(false)]
		public static void CallDefer(bool shouldDefer)
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());
			engine.Defer(shouldDefer);

			Assert.That(engine.ShouldStatementBeDeferred, Is.EqualTo(shouldDefer), nameof(engine.ShouldStatementBeDeferred));
		}

		[TestCase(true)]
		[TestCase(false)]
		public static void CallAgain(bool shouldKeep)
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());
			engine.Again(shouldKeep);

			Assert.That(engine.ShouldStatementBeKept, Is.EqualTo(shouldKeep), nameof(engine.ShouldStatementBeKept));
		}

		[Test]
		public static void CallU()
		{
			const long number = 65;
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(engine.U(number), Is.EqualTo("A"));
		}

		[Test]
		public static void CallUpdateCount()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());
			engine.UpdateCount(1, 2);

			Assert.That(engine.GetCurrentLineCount(), Is.EqualTo(new BigInteger(5)));
		}

		[Test]
		public static void CallUpdateCountWhereLineDoesNotExist()
		{
			var lines = ImmutableArray.Create(new Line(1, 3, _ => { }));
			using var random = new SecureRandom();
			var engine = new ExecutionEngine(lines, random, new StringReader(string.Empty), new StringWriter());

			Assert.That(() => engine.UpdateCount(2, BigInteger.Zero), Throws.TypeOf<KeyNotFoundException>());
		}
	}
}