using NUnit.Framework;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class UnaryUpdateLineCountExpressionSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = (UnaryUpdateLineCountExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.UnaryUpdateLineCountExpression);

		Assert.Multiple(() =>
		{
			Assert.That(((LiteralExpressionSyntax)syntax.LineNumber).Value, Is.EqualTo(BigInteger.One), nameof(syntax.LineNumber));
			Assert.That(syntax.Span.Start, Is.EqualTo(2), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));
		});
	}
}