using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class SyntaxTokenTests
	{
		[Test]
		public static void CreateWithNullValue()
		{
			var kind = SyntaxKind.BangToken;
			var position = 3;
			var text = "a";

			var tree = SyntaxTree.Parse("!");
			var token = new SyntaxToken(tree, kind, position, text, null);

			Assert.Multiple(() =>
			{
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Position, Is.EqualTo(position), nameof(token.Position));
				Assert.That(token.Span.Start, Is.EqualTo(position), nameof(token.Span.Start));
				Assert.That(token.Span.Length, Is.EqualTo(text.Length), nameof(token.Span.Length));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
				Assert.That(token.Value, Is.Null, nameof(token.Value));
			});
		}

		[Test]
		public static void CreateWithValue()
		{
			var kind = SyntaxKind.BangToken;
			var position = 3;
			var text = "a";
			var value = new object();

			var tree = SyntaxTree.Parse("!");
			var token = new SyntaxToken(tree, kind, position, text, value);

			Assert.Multiple(() =>
			{
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Position, Is.EqualTo(position), nameof(token.Position));
				Assert.That(token.Span.Start, Is.EqualTo(position), nameof(token.Span.Start));
				Assert.That(token.Span.Length, Is.EqualTo(text.Length), nameof(token.Span.Length));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
				Assert.That(token.Value, Is.EqualTo(value), nameof(token.Value));
			});
		}
	}
}