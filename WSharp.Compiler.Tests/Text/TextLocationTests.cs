using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Text
{
	public static class TextLocationTests
	{
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
	}
}