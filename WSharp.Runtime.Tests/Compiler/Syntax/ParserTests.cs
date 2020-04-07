using NUnit.Framework;
using System.Collections.Generic;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler.Syntax
{
	public static class ParserTests
	{
		[TestCaseSource(nameof(ParserTests.GetUnaryOperatorPairsData))]
		public static void HonorPrecedenceWhenParsingUnaryExpression((SyntaxKind unaryKind, SyntaxKind binaryKind) value)
		{
			var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(value.unaryKind);
			var binaryPrecedence = SyntaxFacts.GetBinaryOperatorPrecedence(value.binaryKind);
			var unaryText = SyntaxFacts.GetText(value.unaryKind);
			var binaryText = SyntaxFacts.GetText(value.binaryKind);

			var text = $"1 2#({unaryText}3{binaryText}4)";

			Assert.Multiple(() =>
			{
				var expression = ParserTests.ParseExpression(text);

				using var enumerator = new AssertingEnumerator(expression);
				if (unaryPrecedence >= binaryPrecedence)
				{
					enumerator.AssertNode(SyntaxKind.LineStatements);
					enumerator.AssertNode(SyntaxKind.LineStatement);
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "1");
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
					enumerator.AssertNode(SyntaxKind.UpdateLineCountExpression);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "2");
					enumerator.AssertToken(SyntaxKind.UpdateLineCountToken, "#");
					enumerator.AssertNode(SyntaxKind.ParenthesizedExpression);
					enumerator.AssertToken(SyntaxKind.OpenParenthesisToken, "(");
					enumerator.AssertNode(SyntaxKind.BinaryExpression);
					enumerator.AssertNode(SyntaxKind.UnaryExpression);
					enumerator.AssertToken(value.unaryKind, unaryText);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "3");
					enumerator.AssertToken(value.binaryKind, binaryText);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "4");
					enumerator.AssertToken(SyntaxKind.CloseParenthesisToken, ")");
				}
				else
				{
					Assert.Fail("All the unary operators have higher precedence than the binary ones.");
				}
			});
		}

		[TestCaseSource(nameof(ParserTests.GetBinaryOperatorPairsData))]
		public static void HonorPrecedenceWhenParsingBinaryExpression((SyntaxKind operator1, SyntaxKind operator2) value)
		{
			var operator1Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(value.operator1);
			var operator2Precedence = SyntaxFacts.GetBinaryOperatorPrecedence(value.operator2);
			var operator1Text = SyntaxFacts.GetText(value.operator1);
			var operator2Text = SyntaxFacts.GetText(value.operator2);

			var text = $"1 2#(3 {operator1Text} 4 {operator2Text} 5)";

			Assert.Multiple(() =>
			{
				var expression = ParserTests.ParseExpression(text);

				using var enumerator = new AssertingEnumerator(expression);
				if (operator1Precedence >= operator2Precedence)
				{
					enumerator.AssertNode(SyntaxKind.LineStatements);
					enumerator.AssertNode(SyntaxKind.LineStatement);
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "1");
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
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
					enumerator.AssertNode(SyntaxKind.LineStatements);
					enumerator.AssertNode(SyntaxKind.LineStatement);
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
					enumerator.AssertNode(SyntaxKind.LiteralExpression);
					enumerator.AssertToken(SyntaxKind.NumberToken, "1");
					enumerator.AssertNode(SyntaxKind.ExpressionStatement);
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

		private static LineStatementsSyntax ParseExpression(string text)
		{
			var statement = SyntaxTree.Parse(text).Root.LineStatements;
			Assert.That(statement, Is.TypeOf<LineStatementsSyntax>(), nameof(statement));
			return statement;
		}

		public static IEnumerable<(SyntaxKind, SyntaxKind)> GetUnaryOperatorPairsData()
		{
			foreach (var unary in SyntaxFacts.GetUnaryOperatorKinds())
			{
				foreach (var binary in SyntaxFacts.GetBinaryOperatorKinds())
				{
					yield return (unary, binary);
				}
			}
		}

		public static IEnumerable<(SyntaxKind, SyntaxKind)> GetBinaryOperatorPairsData()
		{
			foreach (var operator1 in SyntaxFacts.GetBinaryOperatorKinds())
			{
				foreach (var operator2 in SyntaxFacts.GetBinaryOperatorKinds())
				{
					yield return (operator1, operator2);
				}
			}
		}
	}
}