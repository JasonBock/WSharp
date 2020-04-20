﻿using NUnit.Framework;
using System.Collections.Immutable;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class ParenthesizedExpressionSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var tree = SyntaxTree.Parse("(1)");
			var open = new SyntaxToken(tree, SyntaxKind.OpenParenthesisToken, 0, "(", null);
			var expression = new LiteralExpressionSyntax(tree, new SyntaxToken(tree, SyntaxKind.NumberToken, 1, "1", BigInteger.One));
			var close = new SyntaxToken(tree, SyntaxKind.CloseParenthesisToken, 2, ")", null);

			var syntax = new ParenthesizedExpressionSyntax(tree, open, expression, close);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.ParenthesizedExpression), nameof(syntax.Kind));
				Assert.That(syntax.OpenParenthesisToken, Is.EqualTo(open), nameof(syntax.OpenParenthesisToken));
				Assert.That(syntax.Expression, Is.EqualTo(expression), nameof(syntax.Expression));
				Assert.That(syntax.CloseParenthesisToken, Is.EqualTo(close), nameof(syntax.CloseParenthesisToken));
				Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));

				var children = syntax.GetChildren().ToImmutableArray();
				Assert.That(children.Length, Is.EqualTo(3), nameof(children.Length));
				Assert.That(children[0], Is.EqualTo(open), nameof(children));
				Assert.That(children[1], Is.EqualTo(expression), nameof(children));
				Assert.That(children[2], Is.EqualTo(close), nameof(children));
			});
		}

		[Test]
		public static void CreateWithIncorrectOpenParenthesisToken()
		{
			var tree = SyntaxTree.Parse("-1)");
			var open = new SyntaxToken(tree, SyntaxKind.MinusToken, 0, "-", null);
			var expression = new LiteralExpressionSyntax(tree, new SyntaxToken(tree, SyntaxKind.NumberToken, 1, "1", BigInteger.One));
			var close = new SyntaxToken(tree, SyntaxKind.CloseParenthesisToken, 2, ")", null);

			Assert.That(() => new ParenthesizedExpressionSyntax(tree, open, expression, close), Throws.TypeOf<ParsingException>());
		}

		[Test]
		public static void CreateWithIncorrectCloseParenthesisToken()
		{
			var tree = SyntaxTree.Parse("(1-");
			var open = new SyntaxToken(tree, SyntaxKind.OpenParenthesisToken, 0, "(", null);
			var expression = new LiteralExpressionSyntax(tree, new SyntaxToken(tree, SyntaxKind.NumberToken, 1, "1", BigInteger.One));
			var close = new SyntaxToken(tree, SyntaxKind.MinusToken, 2, "-", null);

			Assert.That(() => new ParenthesizedExpressionSyntax(tree, open, expression, close), Throws.TypeOf<ParsingException>());
		}
	}
}