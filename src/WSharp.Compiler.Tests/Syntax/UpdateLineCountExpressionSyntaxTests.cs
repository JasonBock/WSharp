using NUnit.Framework;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class UpdateLineCountExpressionSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1#2;";
		var syntax = (UpdateLineCountExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.UpdateLineCountExpression);

		Assert.Multiple(() =>
		{
			Assert.That(((LiteralExpressionSyntax)syntax.Left).Value, Is.EqualTo(BigInteger.One), nameof(syntax.Left));
			Assert.That(syntax.OperatorToken.Kind, Is.EqualTo(SyntaxKind.UpdateLineCountToken), nameof(syntax.OperatorToken));
			Assert.That(((LiteralExpressionSyntax)syntax.Right).Value, Is.EqualTo(new BigInteger(2)), nameof(syntax.Right));
			Assert.That(syntax.Span.Start, Is.EqualTo(2), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(5), nameof(syntax.Span.End));
		});
	}
}