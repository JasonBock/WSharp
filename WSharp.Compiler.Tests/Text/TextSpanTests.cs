using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Compiler.Text
{
	public static class TextSpanTests
	{
		[Test]
		public static void CheckEquality()
		{
			var rangeA = new TextSpan(1, 1);
			var rangeB = new TextSpan(2, 2);
			var rangeC = new TextSpan(1, 1);

			Assert.Multiple(() =>
			{
				Assert.That(rangeB, Is.Not.EqualTo(rangeA));
				Assert.That(rangeC, Is.EqualTo(rangeA));
				Assert.That(rangeC, Is.Not.EqualTo(rangeB));

#pragma warning disable CS1718 // Comparison made to same variable
				Assert.That(rangeA == rangeA, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
				Assert.That(rangeA == rangeB, Is.False);
				Assert.That(rangeA == rangeC, Is.True);
				Assert.That(rangeB == rangeC, Is.False);

				Assert.That(rangeA != rangeB, Is.True);
				Assert.That(rangeA != rangeC, Is.False);
				Assert.That(rangeB != rangeC, Is.True);
			});
		}

		[Test]
		public static void CheckEqualityViaEqualsAndObject() => Assert.That(new TextSpan(1, 2).Equals(new object()), Is.False);

		[Test]
		public static void CheckEqualityViaEqualsAndTextSpan() => Assert.That(new TextSpan(1, 2).Equals(new TextSpan(1, 2)), Is.True);

		[Test]
		public static void CheckHashCode()
		{
			var rangeA = new TextSpan(1, 1);
			var rangeB = new TextSpan(1, 2);
			var rangeC = new TextSpan(1, 1);

			Assert.Multiple(() =>
			{
				Assert.That(rangeB.GetHashCode(), Is.Not.EqualTo(rangeA.GetHashCode()));
				Assert.That(rangeC.GetHashCode(), Is.EqualTo(rangeA.GetHashCode()));
			});
		}

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
		public static void DetermineOverlapWhenExists() => 
			Assert.That(TextSpan.FromBounds(1, 3).OverlapsWith(TextSpan.FromBounds(2, 4)), Is.True);

		[Test]
		public static void DetermineOverlapWhenDoesNotExist() =>
			Assert.That(TextSpan.FromBounds(1, 3).OverlapsWith(TextSpan.FromBounds(4, 6)), Is.False);

		[Test]
		public static void CheckToString() =>
			Assert.That(TextSpan.FromBounds(1, 3).ToString(), Is.EqualTo("1..3"));
	}
}