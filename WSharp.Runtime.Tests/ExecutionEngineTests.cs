using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace WSharp.Runtime.Tests
{
	public static class ExecutionEngineTests
	{
		[Test]
		public static void Create()
		{
			var lines = ImmutableList.Create(new Line(1, 1, _ => { }));
			var random = new Random();

			Assert.That(() => new ExecutionEngine(lines, random), Throws.Nothing);
		}

		[Test]
		public static void CreateWhenGivenNullList() =>
			Assert.That(() => new ExecutionEngine(null, new Random()), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenGivenNullRandom() =>
			Assert.That(() => new ExecutionEngine(ImmutableList.Create(new Line(1, 1, _ => { })), null), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenGivenEmptyList()
		{
			var lines = ImmutableList<Line>.Empty;
			Assert.That(() => new ExecutionEngine(lines, new Random()), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public static void CreateWhenListContainsNullEntries()
		{
			var lines = ImmutableList.Create(new Line(1, 1, _ => { }), null, new Line(2, 1, _ => { }), null);
			Assert.That(() => new ExecutionEngine(lines, new Random()), Throws.TypeOf<ExecutionEngineLinesException>());
		}

		[Test]
		public static void CreateWhenListContainsDuplicateIdentifiers()
		{
			var lines = ImmutableList.Create(new Line(1, 1, _ => { }), new Line(1, 1, _ => { }));
			Assert.That(() => new ExecutionEngine(lines, new Random()), Throws.TypeOf<ArgumentException>());
		}

		[TestCase(1ul, 1ul, 1ul, true)]
		[TestCase(1ul, 0ul, 1ul, false)]
		[TestCase(1ul, 1ul, 2ul, false)]
		public static void DoesLineExist(ulong identifier, ulong count, ulong identifierToSearch, bool expectedDoesLineExist)
		{
			var lines = ImmutableList.Create(new Line(identifier, count, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(engine.DoesLineExist(identifierToSearch), Is.EqualTo(expectedDoesLineExist));
		}

		[Test]
		public static void CallGetCurrentLineCount()
		{
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }), new Line(2, 4, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(engine.GetCurrentLineCount(), Is.EqualTo(new BigInteger(7)));
		}

		[Test]
		public static void CallN()
		{
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(engine.N(1), Is.EqualTo(new BigInteger(3)));
		}

		[Test]
		public static void CallNWhereLineDoesNotExist()
		{
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(() => engine.N(2), Throws.TypeOf<KeyNotFoundException>());
		}

		[Test]
		public static void CallU()
		{
			const long number = 3;
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(engine.U(number), Is.EqualTo(number.ToString()));
		}

		[Test]
		public static void CallUpdateCount()
		{
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());
			engine.UpdateCount(1, 2);

			Assert.That(engine.GetCurrentLineCount(), Is.EqualTo(new BigInteger(5)));
		}

		[Test]
		public static void CallUpdateCountWhereLineDoesNotExist()
		{
			var lines = ImmutableList.Create(new Line(1, 3, _ => { }));

			var engine = new ExecutionEngine(lines, new Random());

			Assert.That(() => engine.UpdateCount(2, BigInteger.Zero), Throws.TypeOf<KeyNotFoundException>());
		}
	}
}