using NUnit.Framework;
using System.Linq;
using System.Numerics;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class BinaryExpressionSyntaxTests
	{
		[Test]
		public static void Create()
		{
			var code = "1 1#(1+2);";
 			var syntax = (BinaryExpressionSyntax)SyntaxTree.Parse(code).Root.DescendentNodes()
				.First(_ => _.Kind == SyntaxKind.BinaryExpression);

			Assert.Multiple(() =>
			{
				Assert.That(((LiteralExpressionSyntax)syntax.Left).Value, Is.EqualTo(BigInteger.One), nameof(syntax.Left));
				Assert.That(syntax.OperatorToken.Kind, Is.EqualTo(SyntaxKind.PlusToken), nameof(syntax.OperatorToken));
				Assert.That(((LiteralExpressionSyntax)syntax.Right).Value, Is.EqualTo(new BigInteger(2)), nameof(syntax.Left));
				Assert.That(syntax.Span.Start, Is.EqualTo(5), nameof(syntax.Span.Start));
				Assert.That(syntax.Span.End, Is.EqualTo(8), nameof(syntax.Span.End));
			});
		}
	}
}