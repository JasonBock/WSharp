using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class CompilationUnitSyntaxTests
{
	[Test]
	public static void Create()
	{
		var code = "1 1;";
		var syntax = SyntaxTree.Parse(code).Root;

		Assert.Multiple(() =>
		{
			Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.CompilationUnit), nameof(syntax.Kind));
			Assert.That(syntax.LineStatements.Lines.Count, Is.EqualTo(1), nameof(syntax.LineStatements));
			Assert.That(syntax.EndOfFileToken.Kind, Is.EqualTo(SyntaxKind.EndOfFileToken), nameof(syntax.EndOfFileToken));
			Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
			Assert.That(syntax.Span.End, Is.EqualTo(4), nameof(syntax.Span.End));
		});
	}
}