using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class SyntaxTokenTests
	{
		[Test]
		public static void CreateWithValue()
		{
			var kind = SyntaxKind.NumberToken;
			var position = 0;
			var text = "1";
			var (tokens, _) = SyntaxTree.ParseTokens(text);
			var token = tokens[0];

			Assert.Multiple(() =>
			{
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Position, Is.EqualTo(position), nameof(token.Position));
				Assert.That(token.Span.Start, Is.EqualTo(position), nameof(token.Span.Start));
				Assert.That(token.Span.Length, Is.EqualTo(text.Length), nameof(token.Span.Length));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
				Assert.That(token.Value, Is.Null, nameof(token.Value));
				Assert.That(token.LeadingTrivia.Length, Is.EqualTo(0), nameof(token.LeadingTrivia));
				Assert.That(token.TrailingTrivia.Length, Is.EqualTo(0), nameof(token.TrailingTrivia));
			});
		}

		[Test]
		public static void CreateWithNullValue()
		{
			var kind = SyntaxKind.IdentifierToken;
			var position = 0;
			var text = "a";
			var (tokens, _) = SyntaxTree.ParseTokens(text);
			var token = tokens[0];

			Assert.Multiple(() =>
			{
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Position, Is.EqualTo(position), nameof(token.Position));
				Assert.That(token.Span.Start, Is.EqualTo(position), nameof(token.Span.Start));
				Assert.That(token.Span.Length, Is.EqualTo(text.Length), nameof(token.Span.Length));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
				Assert.That(token.Value, Is.Null, nameof(token.Value));
				Assert.That(token.LeadingTrivia.Length, Is.EqualTo(0), nameof(token.LeadingTrivia));
				Assert.That(token.TrailingTrivia.Length, Is.EqualTo(0), nameof(token.TrailingTrivia));
			});
		}

		[Test]
		public static void CreateWithValueAndTrivia()
		{
			var kind = SyntaxKind.NumberToken;
			var position = 0;
			var text = "/* comment */1/* comment */";
			var (tokens, _) = SyntaxTree.ParseTokens(text);
			var token = tokens[0];

			Assert.Multiple(() =>
			{
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Position, Is.EqualTo(position), nameof(token.Position));
				Assert.That(token.Span.Start, Is.EqualTo(position), nameof(token.Span.Start));
				Assert.That(token.Span.Length, Is.EqualTo(text.Length), nameof(token.Span.Length));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
				Assert.That(token.Value, Is.Null, nameof(token.Value));
				Assert.That(token.LeadingTrivia.Length, Is.EqualTo(0), nameof(token.LeadingTrivia));
				Assert.That(token.TrailingTrivia.Length, Is.EqualTo(0), nameof(token.TrailingTrivia));
			});
		}
	}
}