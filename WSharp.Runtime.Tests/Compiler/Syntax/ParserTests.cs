using NUnit.Framework;
using System.Collections.Generic;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler.Syntax
{
	public static class ParserTests
	{
		[TestCaseSource(nameof(ParserTests.GetBinaryOperatorPairsData))]
		public static void HonorPrecedenceWhenParsingBinaryExpression((SyntaxKind operator1, SyntaxKind operator2) value)
		{
			var operator1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(value.operator1);
			var operator2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(value.operator2);
			var operator1Text = SyntaxFacts.GetText(value.operator1);
			var operator2Text = SyntaxFacts.GetText(value.operator2);

			var text = $"1 2#(3 {operator1Text} 4 {operator2Text} 5)";
			var expression = SyntaxTree.Parse(text).Root;

			Assert.Multiple(() =>
			{
				using var enumerator = new AssertingEnumerator(expression);
				if (operator1Precedence >= operator2Precedence)
				{
					enumerator.AssertNode(SyntaxKind.LineExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "1");
					enumerator.AssertNode(SyntaxKind.UpdateLineCountExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "2");
					enumerator.AssertToken(SyntaxKind.UpdateLineCountToken, "#");
					enumerator.AssertNode(SyntaxKind.ParenthesizedExpression);
					enumerator.AssertToken(SyntaxKind.OpenParenthesisToken, "(");
					enumerator.AssertNode(SyntaxKind.BinaryExpression);
					enumerator.AssertNode(SyntaxKind.BinaryExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "3");
					enumerator.AssertToken(value.operator1, operator1Text);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "4");
					enumerator.AssertToken(value.operator2, operator2Text);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "5");
					enumerator.AssertToken(SyntaxKind.CloseParenthesisToken, ")");
				}
				else
				{
					enumerator.AssertNode(SyntaxKind.LineExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "1");
					enumerator.AssertNode(SyntaxKind.UpdateLineCountExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "2");
					enumerator.AssertToken(SyntaxKind.UpdateLineCountToken, "#");
					enumerator.AssertNode(SyntaxKind.ParenthesizedExpression);
					enumerator.AssertToken(SyntaxKind.OpenParenthesisToken, "(");
					enumerator.AssertNode(SyntaxKind.BinaryExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "3");
					enumerator.AssertToken(value.operator1, operator1Text);
					enumerator.AssertNode(SyntaxKind.BinaryExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "4");
					enumerator.AssertToken(value.operator2, operator2Text);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "5");
					enumerator.AssertToken(SyntaxKind.CloseParenthesisToken, ")");
				}
			});
		}

		public static IEnumerable<(SyntaxKind, SyntaxKind)> GetBinaryOperatorPairsData()
		{
			foreach (var operator1 in SyntaxFacts.GetBinaryOperators())
			{
				foreach (var operator2 in SyntaxFacts.GetBinaryOperators())
				{
					yield return (operator1, operator2);
				}
			}
		}
	}
}