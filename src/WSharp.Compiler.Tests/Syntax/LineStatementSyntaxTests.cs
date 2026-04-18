using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

internal static class LineStatementSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = (LineStatementSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			.First(_ => _.Kind == SyntaxKind.LineStatement);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(syntax.Statements, Has.Count.EqualTo(1), nameof(syntax.Statements));
			Assert.That(syntax.Span.Start, Is.Zero, nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));
		}
	}
}