using NUnit.Framework;
using System.Collections.Immutable;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class LiteralExpressionSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var tree = SyntaxTree.Parse("!");
			var value = new object();
			var token = new SyntaxToken(tree, SyntaxKind.BangToken, 1, "a", value,
				ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);

			var syntax = new LiteralExpressionSyntax(tree, token);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.LiteralExpression), nameof(syntax.Kind));
				Assert.That(syntax.LiteralToken, Is.EqualTo(token), nameof(syntax.LiteralToken));
				Assert.That(syntax.Span.Start, Is.EqualTo(1), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(2), nameof(syntax.Span.End));
				Assert.That(syntax.Value, Is.EqualTo(value), nameof(syntax.Value));

				var children = syntax.GetChildren().ToImmutableArray();
				Assert.That(children.Length, Is.EqualTo(1), nameof(children.Length));
				Assert.That(children[0], Is.EqualTo(token), nameof(children));
			});
		}
	}
}