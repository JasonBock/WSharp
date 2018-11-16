using NUnit.Framework;
using System;

namespace WSharp.Runtime.Tests
{
	public static class LineTests
	{
		[Test]
		public static void Create()
		{
			const ulong identifier = 1;
			const ulong count = 2;
			var code = new Action<IExecutionEngineActions>(_ => { });

			var line = new Line(identifier, count, code);

			Assert.That(line.Identifier, Is.EqualTo(identifier));
			Assert.That(line.Count, Is.EqualTo(count));
			Assert.That(line.Code, Is.SameAs(code));
		}

		[Test]
		public static void CreateWhenCodeIsNull() =>
			Assert.That(() => new Line(default, default, null), Throws.TypeOf<ArgumentNullException>());

		[TestCase(1ul, 2ul, 1ul, 1ul)]
		[TestCase(1ul, 2ul, 3ul, 0ul)]
		public static void UpdateCount(ulong identifier, ulong count, ulong delta, ulong expectedResult)
		{
			var code = new Action<IExecutionEngineActions>(_ => { });

			var line = new Line(identifier, count, code);

			var newLine = line.UpdateCount(delta);
			Assert.That(newLine.Identifier, Is.EqualTo(identifier));
			Assert.That(newLine.Count, Is.EqualTo(expectedResult));
			Assert.That(newLine.Code, Is.SameAs(code));
		}
	}
}
