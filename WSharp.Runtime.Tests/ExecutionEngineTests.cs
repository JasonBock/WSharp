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
			Assert.That(() => new ExecutionEngine(lines), Throws.Nothing);			
		}

		[Test]
		public static void CreateWhenGivenEmptyList()
		{
			var lines = ImmutableList<Line>.Empty;
			Assert.That(() => new ExecutionEngine(lines), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public static void CreateWhenListContainsNullEntries()
		{
			var lines = ImmutableList.Create(new Line(1, 1, _ => { }), null, new Line(2, 1, _ => { }), null);
			Assert.That(() => new ExecutionEngine(lines), Throws.TypeOf<ExecutionEngineLinesException>());
		}

		[Test]
		public static void CreateWhenListContainsDuplicateIdentifiers()
		{
			var lines = ImmutableList.Create(new Line(1, 1, _ => { }), new Line(1, 1, _ => { }));
			Assert.That(() => new ExecutionEngine(lines), Throws.TypeOf<ArgumentException>());
		}
	}
}
