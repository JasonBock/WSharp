﻿using NUnit.Framework;
using WSharp.Compiler.Syntax;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Syntax;

public static class LexerTests
{
	[Test]
	public static void LexerLexesUnterminatedString()
	{
		var text = "\"text";
		var (tokensResult, diagnostics) = SyntaxTree.ParseTokens(text);
		var tokens = tokensResult.ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(tokens.Length, Is.EqualTo(1), $"{nameof(tokens)}.{nameof(tokens.Length)}");
			var token = tokens[0];
			Assert.That(token.Kind, Is.EqualTo(SyntaxKind.StringToken), nameof(token.Kind));
			Assert.That(token.Text, Is.EqualTo(text), nameof(token.Text));
			Assert.That(diagnostics.Length, Is.EqualTo(1), $"{nameof(diagnostics)}.{nameof(diagnostics.Length)}");
			var diagnostic = diagnostics[0];
			Assert.That(diagnostic.Location.Span, Is.EqualTo(new TextSpan(0, 1)), nameof(diagnostic.Location));
			Assert.That(diagnostic.Message, Is.EqualTo("Unterminated string literal."), nameof(diagnostic.Message));
		});
	}

	[Test]
	public static void LexerLexesAllTokens()
	{
		var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
			.Cast<SyntaxKind>()
			.Where(_ => _.IsToken())
			.ToList();

		var testedTokenKinds = LexerTests.GetTokens().Concat(LexerTests.GetSeparators())
			.Select(_ => _.kind);

		var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
		untestedTokenKinds.Remove(SyntaxKind.BadToken);
		untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);
		untestedTokenKinds.ExceptWith(testedTokenKinds);

		Assert.That(untestedTokenKinds, Is.Empty, nameof(untestedTokenKinds));
	}

	[TestCaseSource(nameof(LexerTests.GetTokensData))]
	public static void LexerLexesToken((SyntaxKind kind, string text) value)
	{
		var (tokensResult, _) = SyntaxTree.ParseTokens(value.text);
		var tokens = tokensResult.ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(tokens.Length, Is.EqualTo(1), nameof(tokens.Length));
			var token = tokens[0];
			Assert.That(token.Kind, Is.EqualTo(value.kind), nameof(token.Kind));
			Assert.That(token.Text, Is.EqualTo(value.text), nameof(token.Text));
		});
	}

	[TestCaseSource(nameof(LexerTests.GetSeparatorsData))]
	public static void LexerLexesSeparator((SyntaxKind kind, string text) value)
	{
		var (tokensResult, _) = SyntaxTree.ParseTokens(value.text, true);
		var tokens = tokensResult.ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(tokens.Length, Is.EqualTo(1), nameof(tokens.Length));
			var token = tokens[0];
			Assert.That(token.LeadingTrivia.Length, Is.EqualTo(1), nameof(token.LeadingTrivia));
			var trivia = token.LeadingTrivia[0];
			Assert.That(trivia.Kind, Is.EqualTo(value.kind), nameof(trivia.Kind));
			Assert.That(trivia.Text, Is.EqualTo(value.text), nameof(trivia.Text));
		});
	}

	[TestCaseSource(nameof(LexerTests.GetTokenPairsData))]
	public static void LexerLexesTokenPairs((SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text) value)
	{
		var text = $"{value.t1Text}{value.t2Text}";
		var (tokensResult, _) = SyntaxTree.ParseTokens(text);
		var tokens = tokensResult.ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(tokens.Length, Is.EqualTo(2), nameof(tokens.Length));
			var token1 = tokens[0];
			Assert.That(token1.Kind, Is.EqualTo(value.t1Kind), $"1 - {nameof(token1.Kind)}");
			Assert.That(token1.Text, Is.EqualTo(value.t1Text), $"1 - {nameof(token1.Text)}");
			var token2 = tokens[1];
			Assert.That(token2.Kind, Is.EqualTo(value.t2Kind), $"2 - {nameof(token2.Kind)}");
			Assert.That(token2.Text, Is.EqualTo(value.t2Text), $"2 - {nameof(token2.Text)}");
		});
	}

	[TestCaseSource(nameof(LexerTests.GetTokenPairsWithSeparatorsData))]
	public static void LexerLexesTokenPairsWithSeparators((SyntaxKind t1Kind, string t1Text,
		SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text) value)
	{
		var text = $"{value.t1Text}{value.separatorText}{value.t2Text}";
		var (tokensResult, _) = SyntaxTree.ParseTokens(text);
		var tokens = tokensResult.ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(tokens.Length, Is.EqualTo(2), nameof(tokens.Length));

			var token1 = tokens[0];
			Assert.That(token1.Kind, Is.EqualTo(value.t1Kind), $"1 - {nameof(token1.Kind)}");
			Assert.That(token1.Text, Is.EqualTo(value.t1Text), $"1 - {nameof(token1.Text)}");
			Assert.That(token1.TrailingTrivia.Length, Is.EqualTo(1), $"1 - {nameof(token1.TrailingTrivia)}");

			var separator = token1.TrailingTrivia[0];
			Assert.That(separator.Kind, Is.EqualTo(value.separatorKind), $"Separator - {nameof(separator.Kind)}");
			Assert.That(separator.Text, Is.EqualTo(value.separatorText), $"Separator - {nameof(separator.Text)}");

			var token2 = tokens[1];
			Assert.That(token2.Kind, Is.EqualTo(value.t2Kind), $"2 - {nameof(token2.Kind)}");
			Assert.That(token2.Text, Is.EqualTo(value.t2Text), $"2 - {nameof(token2.Text)}");
		});
	}

	private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators() =>
		new[]
		{
				(SyntaxKind.WhitespaceTrivia, " "),
				(SyntaxKind.WhitespaceTrivia, "  "),
				(SyntaxKind.LineBreakTrivia, "\r"),
				(SyntaxKind.LineBreakTrivia, "\n"),
				(SyntaxKind.LineBreakTrivia, "\r\n"),
				(SyntaxKind.MultiLineCommentTrivia, "/**/")
		};

	private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
	{
		var t1IsKeyword = t1Kind.IsKeyword();
		var t2IsKeyword = t2Kind.IsKeyword();

		return (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken) ||
			(t1Kind == SyntaxKind.NumberToken && t2Kind == SyntaxKind.NumberToken) ||
			(t1Kind == SyntaxKind.StringToken && t2Kind == SyntaxKind.StringToken) ||
			(t1Kind == SyntaxKind.BangToken && t2Kind == SyntaxKind.EqualsEqualsToken) ||
			(t1Kind == SyntaxKind.LessToken && t2Kind == SyntaxKind.EqualsEqualsToken) ||
			(t1Kind == SyntaxKind.GreaterToken && t2Kind == SyntaxKind.EqualsEqualsToken) ||
			(t1Kind == SyntaxKind.AmpersandToken && t2Kind == SyntaxKind.AmpersandToken) ||
			(t1Kind == SyntaxKind.AmpersandToken && t2Kind == SyntaxKind.AmpersandAmpersandToken) ||
			(t1Kind == SyntaxKind.PipeToken && t2Kind == SyntaxKind.PipeToken) ||
			(t1Kind == SyntaxKind.PipeToken && t2Kind == SyntaxKind.PipePipeToken) ||
			(t1Kind == SyntaxKind.SlashToken && t2Kind == SyntaxKind.SlashToken) ||
			(t1Kind == SyntaxKind.SlashToken && t2Kind == SyntaxKind.StarToken) ||
			(t1Kind == SyntaxKind.SlashToken && t2Kind == SyntaxKind.SingleLineCommentTrivia) ||
			(t1Kind == SyntaxKind.SlashToken && t2Kind == SyntaxKind.MultiLineCommentTrivia) ||
			(t1IsKeyword && t2IsKeyword) ||
			(t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken) ||
			(t2IsKeyword && t1Kind == SyntaxKind.IdentifierToken);
	}

	private static IEnumerable<(SyntaxKind kind, string text)> GetTokensData()
	{
		foreach (var (tokenKind, tokenText) in LexerTests.GetTokens())
		{
			yield return (tokenKind, tokenText);
		}
	}

	private static IEnumerable<(SyntaxKind kind, string text)> GetSeparatorsData()
	{
		foreach (var (tokenKind, tokenText) in LexerTests.GetSeparators())
		{
			yield return (tokenKind, tokenText);
		}
	}

	private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairsData()
	{
		foreach (var (t1Kind, t1Text, t2Kind, t2Text) in LexerTests.GetTokenPairs())
		{
			yield return (t1Kind, t1Text, t2Kind, t2Text);
		}
	}

	private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparatorsData()
	{
		foreach (var (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text) in LexerTests.GetTokenPairsWithSeparators())
		{
			yield return (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text);
		}
	}

	private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
	{
		var fixedTokens = Enum.GetValues(typeof(SyntaxKind))
			.Cast<SyntaxKind>()
			.Select(kind => (kind, text: SyntaxFacts.GetText(kind)))
			.Where(_ => !string.IsNullOrWhiteSpace(_.text));

		var dynamicTokens = new[]
		{
				(SyntaxKind.IdentifierToken, "a"),
				(SyntaxKind.IdentifierToken, "abc"),
				(SyntaxKind.NumberToken, "1"),
				(SyntaxKind.NumberToken, "123"),
				(SyntaxKind.StringToken, "\"Test\""),
				(SyntaxKind.StringToken, "\"Te\"\"st\""),
			};

		return fixedTokens.Concat(dynamicTokens);
	}

	private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
	{
		foreach (var (t1Kind, t1Text) in LexerTests.GetTokens())
		{
			foreach (var (t2Kind, t2Text) in LexerTests.GetTokens())
			{
				if (!LexerTests.RequiresSeparator(t1Kind, t2Kind))
				{
					yield return (t1Kind, t1Text, t2Kind, t2Text);
				}
			}
		}
	}

	private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparators()
	{
		foreach (var (t1Kind, t1Text) in LexerTests.GetTokens())
		{
			foreach (var (t2Kind, t2Text) in LexerTests.GetTokens())
			{
				if (LexerTests.RequiresSeparator(t1Kind, t2Kind))
				{
					foreach (var (separatorKind, separatorText) in LexerTests.GetSeparators())
					{
						if (!LexerTests.RequiresSeparator(t1Kind, separatorKind) &&
							!LexerTests.RequiresSeparator(separatorKind, t2Kind))
						{
							yield return (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text);
						}
					}
				}
			}
		}
	}
}