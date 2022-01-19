using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Text;

public static class TextLocationTests
{
	[Test]
	public static void Equality()
	{
		var code = "1 1;";

		var text1 = SourceText.From(code);
		var text2 = SourceText.From(code);

		var span1 = new TextSpan(2, 1);
		var span2 = new TextSpan(1, 1);

		var location1 = new TextLocation(text1, span1);
		var location2 = new TextLocation(text1, span2);
		var location3 = new TextLocation(text2, span1);
		var location4 = new TextLocation(text2, span2);
		var location5 = new TextLocation(text1, span1);

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
		Assert.Multiple(() =>
		{
			Assert.That(location1, Is.Not.EqualTo(location2), "location1.Equals(location2)");
			Assert.That(location1 == location2, Is.False, "location1 == location2");
			Assert.That(location1 != location2, Is.True, "location1 != location2");

			Assert.That(location1, Is.Not.EqualTo(location3), "location1.Equals(location3)");
			Assert.That(location1 == location3, Is.False, "location1 == location3");
			Assert.That(location1 != location3, Is.True, "location1 != location3");

			Assert.That(location1, Is.Not.EqualTo(location4), "location1.Equals(location4)");
			Assert.That(location1 == location4, Is.False, "location1 == location4");
			Assert.That(location1 != location4, Is.True, "location1 != location4");

			Assert.That(location1, Is.EqualTo(location5), "location1.Equals(location5)");
			Assert.That(location1 == location5, Is.True, "location1 == location5");
			Assert.That(location1 != location5, Is.False, "location1 != location5");
		});
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
	}

	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var text = SourceText.From(code);
		var span = new TextSpan(2, 1);
		var location = new TextLocation(text, span);

		Assert.Multiple(() =>
		{
			Assert.That(location.EndCharacter, Is.EqualTo(3), nameof(location.EndCharacter));
			Assert.That(location.EndLine, Is.EqualTo(0), nameof(location.EndLine));
			Assert.That(location.Span, Is.EqualTo(span), nameof(location.Span));
			Assert.That(location.StartCharacter, Is.EqualTo(2), nameof(location.StartCharacter));
			Assert.That(location.StartLine, Is.EqualTo(0), nameof(location.StartLine));
			Assert.That(location.Text, Is.EqualTo(text), nameof(location.Text));
		});
	}

	[Test]
	public static void CreateWithMultipleLines()
	{
		var code = $"1 1;{Environment.NewLine}2 2;";
		var text = SourceText.From(code);
		var span = new TextSpan(2, 7);
		var location = new TextLocation(text, span);

		Assert.Multiple(() =>
		{
			Assert.That(location.EndCharacter, Is.EqualTo(9), nameof(location.EndCharacter));
			Assert.That(location.EndLine, Is.EqualTo(1), nameof(location.EndLine));
			Assert.That(location.Span, Is.EqualTo(span), nameof(location.Span));
			Assert.That(location.StartCharacter, Is.EqualTo(2), nameof(location.StartCharacter));
			Assert.That(location.StartLine, Is.EqualTo(0), nameof(location.StartLine));
			Assert.That(location.Text, Is.EqualTo(text), nameof(location.Text));
		});
	}
}