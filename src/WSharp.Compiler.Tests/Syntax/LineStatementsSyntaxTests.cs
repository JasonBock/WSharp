using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class LineStatementsSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = (LineStatementsSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.LineStatements);

		Assert.Multiple(() =>
		{
			Assert.That(syntax.Lines.Count, Is.EqualTo(1), nameof(syntax.Lines));
			Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));
		});
	}
}