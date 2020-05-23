using NUnit.Framework;
using System.Linq;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax
{
	public static class ExpressionStatementSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var code = "1 1;";
			var syntax = (ExpressionStatementSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			  .First(_ => _.Kind == SyntaxKind.ExpressionStatement);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Expression.Kind, Is.EqualTo(SyntaxKind.LiteralExpression), nameof(syntax.Expression));
				Assert.That(syntax.Span.Start, Is.EqualTo(0), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(1), nameof(syntax.Span.End));
			});
		}
	}
}