using NUnit.Framework;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class BinaryExpressionSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var tree = SyntaxTree.Parse("1+2");
			var left = new LiteralExpressionSyntax(tree, 
				new SyntaxToken(tree, SyntaxKind.NumberToken, 0, "1", BigInteger.One, 
					ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty));
			var @operator = new SyntaxToken(tree, SyntaxKind.PlusToken, 1, "+", null,
				ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);
			var right = new LiteralExpressionSyntax(tree, 
				new SyntaxToken(tree, SyntaxKind.NumberToken, 2, "2", BigInteger.Parse("2"),
					ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty));

			var syntax = new BinaryExpressionSyntax(tree, left, @operator, right);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.BinaryExpression), nameof(syntax.Kind));
				Assert.That(syntax.Left, Is.EqualTo(left), nameof(syntax.Left));
				Assert.That(syntax.OperatorToken, Is.EqualTo(@operator), nameof(syntax.OperatorToken));
				Assert.That(syntax.Right, Is.EqualTo(right), nameof(syntax.Right));
				Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));

				var children = syntax.GetChildren().ToImmutableArray();
				Assert.That(children.Length, Is.EqualTo(3), nameof(children.Length));
				Assert.That(children[0], Is.EqualTo(left), nameof(children));
				Assert.That(children[1], Is.EqualTo(@operator), nameof(children));
				Assert.That(children[2], Is.EqualTo(right), nameof(children));
			});
		}

		// TODO: Add test for creating with trivia.
	}
}