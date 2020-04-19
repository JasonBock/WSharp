using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Compiler.Text
{
	public static class TextLineTests
	{
		[Test]
		public static void Create()
		{
			var source = SourceText.From("1 2#3;");
			var line = new TextLine(source, 0, 6, 6);
			Assert.That(line.Start, Is.EqualTo(0), nameof(line.Start));
			Assert.That(line.Length, Is.EqualTo(6), nameof(line.Length));
			Assert.That(line.LengthIncludingLineBreak, Is.EqualTo(6), nameof(line.LengthIncludingLineBreak));
			Assert.That(line.End, Is.EqualTo(6), nameof(line.End));
			Assert.That(line.Text, Is.EqualTo(source), nameof(line.Text));
			Assert.That(line.Span, Is.EqualTo(new TextSpan(0, 6)), nameof(line.Span));
			Assert.That(line.SpanIncludingLineBreak, Is.EqualTo(new TextSpan(0, 6)), nameof(line.SpanIncludingLineBreak));
		}
	}
}