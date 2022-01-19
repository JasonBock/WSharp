using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class UnaryExpressionSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 -1;";
		var syntax = (UnaryExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.UnaryExpression);

		Assert.Multiple(() =>
		{
			Assert.That(syntax.OperatorToken.Kind, Is.EqualTo(SyntaxKind.MinusToken), nameof(syntax.OperatorToken));
			Assert.That(syntax.Operand.Kind, Is.EqualTo(SyntaxKind.LiteralExpression), nameof(syntax.Operand));
			Assert.That(syntax.Span.Start, Is.EqualTo(2), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(4), nameof(syntax.Span.End));
		});
	}
}