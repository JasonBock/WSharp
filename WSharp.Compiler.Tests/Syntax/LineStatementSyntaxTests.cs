using NUnit.Framework;
using System.Linq;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax
{
	public static class LineStatementSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var code = "1 1;";
			var syntax = (LineStatementSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			  .First(_ => _.Kind == SyntaxKind.LineStatement);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Statements.Count, Is.EqualTo(1), nameof(syntax.Statements));
				Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(3), nameof(syntax.Span.End));
			});
		}
	}
}