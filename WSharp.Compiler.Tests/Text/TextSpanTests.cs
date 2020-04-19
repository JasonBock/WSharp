using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Compiler.Text
{
	public static class TextSpanTests
	{
		[Test]
		public static void Create()
		{
			var span = new TextSpan();

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(0), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(0), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(0), nameof(span.End));
			});
		}

		[Test]
		public static void CreateWithValues()
		{
			var span = new TextSpan(1, 2);

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(1), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(2), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(3), nameof(span.End));
			});
		}

		[Test]
		public static void CreateFromBounds()
		{
			var span = TextSpan.FromBounds(1, 4);

			Assert.Multiple(() =>
			{
				Assert.That(span.Start, Is.EqualTo(1), nameof(span.Start));
				Assert.That(span.Length, Is.EqualTo(3), nameof(span.Length));
				Assert.That(span.End, Is.EqualTo(4), nameof(span.End));
			});
		}

		[Test]
		public static void CompareWhenStartIsGreater()
		{
			var target = new TextSpan(2, 1);
			var other = new TextSpan(1, 1);

			Assert.That(target.CompareTo(other), Is.GreaterThan(0));
		}

		[Test]
		public static void CompareWhenStartIsLesser()
		{
			var target = new TextSpan(1, 1);
			var other = new TextSpan(2, 1);

			Assert.That(target.CompareTo(other), Is.LessThan(0));
		}

		[Test]
		public static void CompareWhenLengthIsGreater()
		{
			var target = new TextSpan(1, 2);
			var other = new TextSpan(1, 1);

			Assert.That(target.CompareTo(other), Is.GreaterThan(0));
		}

		[Test]
		public static void CompareWhenLengthIsLesser()
		{
			var target = new TextSpan(1, 1);
			var other = new TextSpan(1, 2);

			Assert.That(target.CompareTo(other), Is.LessThan(0));
		}

		[Test]
		public static void CompareWhenEqual()
		{
			var target = new TextSpan(1, 1);
			var other = new TextSpan(1, 1);

			Assert.That(target.CompareTo(other), Is.EqualTo(0));
		}
	}
}