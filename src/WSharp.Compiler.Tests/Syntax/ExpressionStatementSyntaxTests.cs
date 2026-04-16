using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

internal static class ExpressionStatementSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = (ExpressionStatementSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.ExpressionStatement);

	  using (Assert.EnterMultipleScope())
	  {
			Assert.That(syntax.Expression.Kind, Is.EqualTo(SyntaxKind.LiteralExpression), nameof(syntax.Expression));
			Assert.That(syntax.Span.Start, Is.Zero, nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(1), nameof(syntax.Span.End));
		}
	}
}