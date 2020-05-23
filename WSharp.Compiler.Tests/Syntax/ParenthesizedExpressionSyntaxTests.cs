using NUnit.Framework;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class ParenthesizedExpressionSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var code = "1 1#(1+2);";
			var syntax = (ParenthesizedExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
			  .First(_ => _.Kind == SyntaxKind.ParenthesizedExpression);

			Assert.Multiple(() =>
			{
				Assert.That(syntax.Kind, Is.EqualTo(SyntaxKind.ParenthesizedExpression), nameof(syntax.Kind));
				Assert.That(syntax.OpenParenthesisToken.Kind, Is.EqualTo(SyntaxKind.OpenParenthesisToken), nameof(syntax.OpenParenthesisToken));
				Assert.That(syntax.Expression.Kind, Is.EqualTo(SyntaxKind.BinaryExpression), nameof(syntax.Expression));
				Assert.That(syntax.CloseParenthesisToken.Kind, Is.EqualTo(SyntaxKind.CloseParenthesisToken), nameof(syntax.CloseParenthesisToken));
				Assert.That(syntax.Span.Start, Is.EqualTo(4), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(9), nameof(syntax.Span.End));
			});
		}
	}
}