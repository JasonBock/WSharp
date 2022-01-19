using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Text;

public static class TextSpanTests
{
	[Test]
	public static void CheckEquality()
	{
		var rangeA = new TextSpan(1, 1);
		var rangeB = new TextSpan(2, 2);
		var rangeC = new TextSpan(1, 1);

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
		Assert.Multiple(() =>
		{
			Assert.That(rangeB, Is.Not.EqualTo(rangeA), "rangeB.Equals(rangeA)");
			Assert.That(rangeB == rangeA, Is.False, "rangeB == rangeA");
			Assert.That(rangeB != rangeA, Is.True, "rangeB != rangeA");

			Assert.That(rangeC, Is.EqualTo(rangeA));
			Assert.That(rangeC == rangeA, Is.True, "rangeC == rangeA");
			Assert.That(rangeC != rangeA, Is.False, "rangeC != rangeA");

			Assert.That(rangeC, Is.Not.EqualTo(rangeB));
			Assert.That(rangeC == rangeB, Is.False, "rangeC == rangeB");
			Assert.That(rangeC != rangeB, Is.True, "rangeC != rangeB");

			Assert.That(rangeA, Is.Not.EqualTo(rangeB));
			Assert.That(rangeA == rangeB, Is.False, "rangeA == rangeB");
			Assert.That(rangeA != rangeB, Is.True, "rangeA != rangeB");

			Assert.That(rangeA, Is.EqualTo(rangeC));
			Assert.That(rangeA == rangeC, Is.True, "rangeA == rangeC");
			Assert.That(rangeA != rangeC, Is.False, "rangeA != rangeC");

			Assert.That(rangeB, Is.Not.EqualTo(rangeC));
			Assert.That(rangeB == rangeC, Is.False, "rangeB == rangeC");
			Assert.That(rangeB != rangeC, Is.True, "rangeB != rangeC");
		});
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
	}

	[Test]
	public static void CheckEqualityViaEqualsAndObject() => Assert.That(new TextSpan(1, 2), Is.Not.EqualTo(new object()));

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
	public static void OverlapsWith()
	{
		var span1 = TextSpan.FromBounds(1, 4);
		var span2 = TextSpan.FromBounds(3, 8);
		var span3 = TextSpan.FromBounds(6, 12);

		Assert.Multiple(() =>
		{
			Assert.That(span1.OverlapsWith(span2), Is.True, "span1.OverlapsWith(span2)");
			Assert.That(span1.OverlapsWith(span3), Is.False, "span1.OverlapsWith(span3)");
			Assert.That(span2.OverlapsWith(span3), Is.True, "span2.OverlapsWith(span3)");
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