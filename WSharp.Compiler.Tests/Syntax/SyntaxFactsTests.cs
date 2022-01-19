using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Syntax;

public static class SyntaxFactsTests
{
	[TestCaseSource(nameof(SyntaxFactsTests.GetSyntaxKindData))]
	public static void RoundTripSyntaxKind(SyntaxKind kind)
	{
		var text = SyntaxFacts.GetText(kind);

		if (!string.IsNullOrWhiteSpace(text))
		{
			var (tokensResult, _) = SyntaxTree.ParseTokens(text);
			var tokens = tokensResult.ToArray();

			Assert.Multiple(() =>
			{
				Assert.That(tokens.Length, Is.EqualTo(1), nameof(tokens.Length));
				var token = tokens[0];
				Assert.That(token.Kind, Is.EqualTo(kind), nameof(token.Kind));
				Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
			});
		}
	}

	[Test]
	public static void GetUnaryOperatorKinds()
	{
		var operatorKinds = SyntaxFacts.GetUnaryOperatorKinds().ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(operatorKinds.Length, Is.EqualTo(4), nameof(operatorKinds.Length));
			Assert.That(operatorKinds, Contains.Item(SyntaxKind.PlusToken), nameof(SyntaxKind.PlusToken));
			Assert.That(operatorKinds, Contains.Item(SyntaxKind.MinusToken), nameof(SyntaxKind.MinusToken));
			Assert.That(operatorKinds, Contains.Item(SyntaxKind.BangToken), nameof(SyntaxKind.BangToken));
			Assert.That(operatorKinds, Contains.Item(SyntaxKind.TildeToken), nameof(SyntaxKind.TildeToken));
		});
	}

	[Test]
	public static void GetBinaryOperatorKinds()
	{
		var binaryKinds = SyntaxFacts.GetBinaryOperatorKinds().ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(binaryKinds.Length, Is.EqualTo(16), nameof(binaryKinds.Length));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.StarToken), nameof(SyntaxKind.StarToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.SlashToken), nameof(SyntaxKind.SlashToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.PercentToken), nameof(SyntaxKind.PercentToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.PlusToken), nameof(SyntaxKind.PlusToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.MinusToken), nameof(SyntaxKind.MinusToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.EqualsEqualsToken), nameof(SyntaxKind.EqualsEqualsToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.BangEqualsToken), nameof(SyntaxKind.BangEqualsToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.LessToken), nameof(SyntaxKind.LessToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.LessOrEqualsToken), nameof(SyntaxKind.LessOrEqualsToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.GreaterToken), nameof(SyntaxKind.GreaterToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.GreaterOrEqualsToken), nameof(SyntaxKind.GreaterOrEqualsToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.AmpersandToken), nameof(SyntaxKind.AmpersandToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.AmpersandAmpersandToken), nameof(SyntaxKind.AmpersandAmpersandToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.PipeToken), nameof(SyntaxKind.PipeToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.PipePipeToken), nameof(SyntaxKind.PipePipeToken));
			Assert.That(binaryKinds, Contains.Item(SyntaxKind.HatToken), nameof(SyntaxKind.HatToken));
		});
	}

	[TestCase(SyntaxKind.SingleLineCommentTrivia, true)]
	[TestCase(SyntaxKind.MultiLineCommentTrivia, true)]
	[TestCase(SyntaxKind.LiteralExpression, false)]
	public static void IsComment(SyntaxKind kind, bool expectedResult) =>
		Assert.That(kind.IsComment(), Is.EqualTo(expectedResult));

	[TestCase(SyntaxKind.FalseKeyword, true)]
	[TestCase(SyntaxKind.LiteralExpression, false)]
	public static void IsKeyword(SyntaxKind kind, bool expectedResult) =>
		Assert.That(kind.IsKeyword(), Is.EqualTo(expectedResult));

	[TestCase(SyntaxKind.SkippedTextTrivia, true)]
	[TestCase(SyntaxKind.LineBreakTrivia, true)]
	[TestCase(SyntaxKind.WhitespaceTrivia, true)]
	[TestCase(SyntaxKind.SingleLineCommentTrivia, true)]
	[TestCase(SyntaxKind.MultiLineCommentTrivia, true)]
	[TestCase(SyntaxKind.BadToken, true)]
	[TestCase(SyntaxKind.LiteralExpression, false)]
	public static void IsTrivia(SyntaxKind kind, bool expectedResult) =>
		Assert.That(kind.IsTrivia(), Is.EqualTo(expectedResult));

	[TestCase(SyntaxKind.FalseKeyword, true)]
	[TestCase(SyntaxKind.AmpersandToken, true)]
	[TestCase(SyntaxKind.SkippedTextTrivia, false)]
	[TestCase(SyntaxKind.LiteralExpression, false)]
	public static void IsToken(SyntaxKind kind, bool expectedResult) =>
		Assert.That(kind.IsToken(), Is.EqualTo(expectedResult));

	private static IEnumerable<SyntaxKind> GetSyntaxKindData()
	{
		foreach (var kind in (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind)))
		{
			yield return kind;
		}
	}
}