using NUnit.Framework;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

internal static class LiteralExpressionSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 2;";
		var syntax = (LiteralExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.LiteralExpression);

	  using (Assert.EnterMultipleScope())
	  {
			Assert.That(syntax.LiteralToken.Kind, Is.EqualTo(SyntaxKind.NumberToken), nameof(syntax.LiteralToken));
			Assert.That(syntax.Value, Is.EqualTo(BigInteger.One), nameof(syntax.Value));
			Assert.That(syntax.Span.Start, Is.Zero, nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(1), nameof(syntax.Span.End));
		}
	}
}