using NUnit.Framework;
using System;
using System.Numerics;

namespace WSharp.Runtime.Tests
{
	public static class LineTests
	{
		[Test]
		public static void Create()
		{
			const ulong identifier = 1;
			var count = new BigInteger(2);
			var code = new Action<IExecutionEngineActions>(_ => { });

			var line = new Line(identifier, count, code);

			Assert.That(line.Identifier, Is.EqualTo(identifier));
			Assert.That(line.Count, Is.EqualTo(count));
			Assert.That(line.Code, Is.SameAs(code));
		}

		[Test]
		public static void CreateWhenCodeIsNull() =>
			Assert.That(() => new Line(default, default, null), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void CreateWhenCountIsLessThanZero() =>
			Assert.That(() => new Line(1ul, -1, new Action<IExecutionEngineActions>(_ => { })), Throws.TypeOf<ArgumentException>());

		[TestCase(1ul, 2ul, 1L, 3ul)]
		[TestCase(1ul, 2ul, -1L, 1ul)]
		[TestCase(1ul, 2ul, -3L, 0ul)]
		public static void UpdateCount(ulong identifier, ulong count, long delta, ulong expectedResult)
		{
			var code = new Action<IExecutionEngineActions>(_ => { });

			var line = new Line(identifier, count, code);

			var newLine = line.UpdateCount(delta);
			Assert.That(newLine.Identifier, Is.EqualTo(identifier));
			Assert.That(newLine.Count, Is.EqualTo(new BigInteger(expectedResult)));
			Assert.That(newLine.Code, Is.SameAs(code));
		}
	}
}
