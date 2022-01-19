using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class CallExpressionSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 print(\"hi\");";
		var syntax = (CallExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.CallExpression);

		Assert.Multiple(() =>
		{
			Assert.That(syntax.Identifier.Kind, Is.EqualTo(SyntaxKind.IdentifierToken), nameof(syntax.Identifier));
			Assert.That(syntax.Identifier.Text, Is.EqualTo("print"), nameof(syntax.Identifier));
			Assert.That(syntax.OpenParenthesisToken.Kind, Is.EqualTo(SyntaxKind.OpenParenthesisToken), nameof(syntax.OpenParenthesisToken));
			Assert.That(syntax.CloseParenthesisToken.Kind, Is.EqualTo(SyntaxKind.CloseParenthesisToken), nameof(syntax.CloseParenthesisToken));
			Assert.That(syntax.Arguments.Count, Is.EqualTo(1), nameof(syntax.Arguments.Count));
			Assert.That(syntax.Arguments[0].Kind, Is.EqualTo(SyntaxKind.LiteralExpression), nameof(syntax.Arguments));
			Assert.That(((LiteralExpressionSyntax)syntax.Arguments[0]).Value, Is.EqualTo("hi"), nameof(syntax.Arguments));
			Assert.That(syntax.Span.Start, Is.EqualTo(2), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(13), nameof(syntax.Span.End));
		});
	}
}