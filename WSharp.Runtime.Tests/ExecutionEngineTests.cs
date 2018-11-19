using NUnit.Framework;
using System;
using System.Collections.Immutable;

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
			var random = new Random();

			var engine = new ExecutionEngine(lines, random);

			Assert.That(engine.DoesLineExist(identifierToSearch), Is.EqualTo(expectedDoesLineExist));
		}
	}
}