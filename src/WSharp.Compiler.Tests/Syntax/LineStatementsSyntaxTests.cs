using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

internal static class LineStatementsSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = (LineStatementsSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.LineStatements);

	  using (Assert.EnterMultipleScope())
	  {
			Assert.That(syntax.Lines, Has.Count.EqualTo(1), nameof(syntax.Lines));
			Assert.That(syntax.Span.Start, Is.Zero, nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));
		}
	}
}