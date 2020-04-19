using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Compiler.Text
{
	public static class SourceTextTests
	{
		[Test]
		public static void Create()
		{
			var text = "1 2#3;";
			var source = SourceText.From(text);

			Assert.Multiple(() =>
			{
				Assert.That(source.Lines.Length, Is.EqualTo(1), nameof(source.Lines));
				var line = source.Lines[0];
				Assert.That(line.Start, Is.EqualTo(0), nameof(line.Start));
				Assert.That(line.Length, Is.EqualTo(6), nameof(line.Length));
				Assert.That(line.LengthIncludingLineBreak, Is.EqualTo(6), nameof(line.LengthIncludingLineBreak));
				Assert.That(line.End, Is.EqualTo(6), nameof(line.End));
				Assert.That(line.Text, Is.EqualTo(source), nameof(line.Text));
				Assert.That(line.Span, Is.EqualTo(new TextSpan(0, 6)), nameof(line.Span));
				Assert.That(line.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(0, 6)), nameof(line.SpanIncludingLineBreak));
			});
		}

		[Test]
		public static void CreateWithLineFeed()
		{
			var text = "1 2#3;\r";
			var source = SourceText.From(text);

			Assert.Multiple(() =>
			{
				Assert.That(source.Lines.Length, Is.EqualTo(2), nameof(source.Lines));
				var line1 = source.Lines[0];
				Assert.That(line1.Start, Is.EqualTo(0), $"{nameof(line1)} - {nameof(line1.Start)}");
				Assert.That(line1.Length, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.Length)}");
				Assert.That(line1.LengthIncludingLineBreak, Is.EqualTo(7), $"{nameof(line1)} - {nameof(line1.LengthIncludingLineBreak)}");
				Assert.That(line1.End, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.End)}");
				Assert.That(line1.Text, Is.EqualTo(source), $"{nameof(line1)} - {nameof(line1.Text)}");
				Assert.That(line1.Span, Is.EqualTo(new TextSpan(0, 6)), $"{nameof(line1)} - {nameof(line1.Span)}");
				Assert.That(line1.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(0, 7)), $"{nameof(line1)} - {nameof(line1.SpanIncludingLineBreak)}");

				var line2 = source.Lines[1];
				Assert.That(line2.Start, Is.EqualTo(7), $"{nameof(line2)} - {nameof(line2.Start)}");
				Assert.That(line2.Length, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.Length)}");
				Assert.That(line2.LengthIncludingLineBreak, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.LengthIncludingLineBreak)}");
				Assert.That(line2.End, Is.EqualTo(7), $"{nameof(line2)} - {nameof(line2.End)}");
				Assert.That(line2.Text, Is.EqualTo(source), $"{nameof(line2)} - {nameof(line2.Text)}");
				Assert.That(line2.Span, Is.EqualTo(new TextSpan(7, 0)), $"{nameof(line2)} - {nameof(line2.Span)}");
				Assert.That(line2.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(7, 0)), $"{nameof(line2)} - {nameof(line2.SpanIncludingLineBreak)}");
			});
		}

		[Test]
		public static void CreateWithCarriageReturn()
		{
			var text = "1 2#3;\n";
			var source = SourceText.From(text);

			Assert.Multiple(() =>
			{
				Assert.That(source.Lines.Length, Is.EqualTo(2), nameof(source.Lines));
				var line1 = source.Lines[0];
				Assert.That(line1.Start, Is.EqualTo(0), $"{nameof(line1)} - {nameof(line1.Start)}");
				Assert.That(line1.Length, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.Length)}");
				Assert.That(line1.LengthIncludingLineBreak, Is.EqualTo(7), $"{nameof(line1)} - {nameof(line1.LengthIncludingLineBreak)}");
				Assert.That(line1.End, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.End)}");
				Assert.That(line1.Text, Is.EqualTo(source), $"{nameof(line1)} - {nameof(line1.Text)}");
				Assert.That(line1.Span, Is.EqualTo(new TextSpan(0, 6)), $"{nameof(line1)} - {nameof(line1.Span)}");
				Assert.That(line1.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(0, 7)), $"{nameof(line1)} - {nameof(line1.SpanIncludingLineBreak)}");

				var line2 = source.Lines[1];
				Assert.That(line2.Start, Is.EqualTo(7), $"{nameof(line2)} - {nameof(line2.Start)}");
				Assert.That(line2.Length, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.Length)}");
				Assert.That(line2.LengthIncludingLineBreak, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.LengthIncludingLineBreak)}");
				Assert.That(line2.End, Is.EqualTo(7), $"{nameof(line2)} - {nameof(line2.End)}");
				Assert.That(line2.Text, Is.EqualTo(source), $"{nameof(line2)} - {nameof(line2.Text)}");
				Assert.That(line2.Span, Is.EqualTo(new TextSpan(7, 0)), $"{nameof(line2)} - {nameof(line2.Span)}");
				Assert.That(line2.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(7, 0)), $"{nameof(line2)} - {nameof(line2.SpanIncludingLineBreak)}");
			});
		}

		[Test]
		public static void CreateWithLineFeedCarriageReturn()
		{
			var text = "1 2#3;\r\n";
			var source = SourceText.From(text);

			Assert.Multiple(() =>
			{
				Assert.That(source.Lines.Length, Is.EqualTo(2), nameof(source.Lines));
				var line1 = source.Lines[0];
				Assert.That(line1.Start, Is.EqualTo(0), $"{nameof(line1)} - {nameof(line1.Start)}");
				Assert.That(line1.Length, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.Length)}");
				Assert.That(line1.LengthIncludingLineBreak, Is.EqualTo(8), $"{nameof(line1)} - {nameof(line1.LengthIncludingLineBreak)}");
				Assert.That(line1.End, Is.EqualTo(6), $"{nameof(line1)} - {nameof(line1.End)}");
				Assert.That(line1.Text, Is.EqualTo(source), $"{nameof(line1)} - {nameof(line1.Text)}");
				Assert.That(line1.Span, Is.EqualTo(new TextSpan(0, 6)), $"{nameof(line1)} - {nameof(line1.Span)}");
				Assert.That(line1.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(0, 8)), $"{nameof(line1)} - {nameof(line1.SpanIncludingLineBreak)}");

				var line2 = source.Lines[1];
				Assert.That(line2.Start, Is.EqualTo(8), $"{nameof(line2)} - {nameof(line2.Start)}");
				Assert.That(line2.Length, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.Length)}");
				Assert.That(line2.LengthIncludingLineBreak, Is.EqualTo(0), $"{nameof(line2)} - {nameof(line2.LengthIncludingLineBreak)}");
				Assert.That(line2.End, Is.EqualTo(8), $"{nameof(line2)} - {nameof(line2.End)}");
				Assert.That(line2.Text, Is.EqualTo(source), $"{nameof(line2)} - {nameof(line2.Text)}");
				Assert.That(line2.Span, Is.EqualTo(new TextSpan(8, 0)), $"{nameof(line2)} - {nameof(line2.Span)}");
				Assert.That(line2.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(8, 0)), $"{nameof(line2)} - {nameof(line2.SpanIncludingLineBreak)}");
			});
		}
	}
}