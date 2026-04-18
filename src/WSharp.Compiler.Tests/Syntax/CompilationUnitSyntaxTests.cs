using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

internal static class CompilationUnitSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = SyntaxTree.Parse(code).Root;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.CompilationUnit), nameof(syntax.Kind));
			Assert.That(syntax.LineStatements.Lines, Has.Count.EqualTo(1), nameof(syntax.LineStatements));
			Assert.That(syntax.EndOfFileToken.Kind, Is.EqualTo(SyntaxKind.EndOfFileToken), nameof(syntax.EndOfFileToken));
			Assert.That(syntax.Span.Start, Is.Zero, nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(4), nameof(syntax.Span.End));
		}
	}
}