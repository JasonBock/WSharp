using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler.Syntax
{
	public static class LexerTests
	{
		private static IEnumerable<SyntaxToken> ParseTokens(string text)
		{
			var lexer = new Lexer(text);

			while(true)
			{
				var token = lexer.Lex();
				
				if(token.Kind != SyntaxKind.EndOfFileToken)
				{
					yield return token;
				}
				else
				{
					break;
				}
			}
		}

		[TestCaseSource(nameof(GetTokens))]
		public static void TestLexer((SyntaxKind kind, string text) value)
		{
			var tokens = LexerTests.ParseTokens(value.text).ToArray();

			Assert.Multiple(() =>
			{
				Assert.That(tokens.Length, Is.EqualTo(1), nameof(tokens.Length));
				var token = tokens[0];
				Assert.That(token.Kind, Is.EqualTo(value.kind), nameof(token.Kind));
				Assert.That(token.Text, Is.EqualTo(value.text), nameof(token.Text));
			});
		}

		private static IEnumerable<(SyntaxKind kind, string text)> GetTokens() => 
			new[]
			{
				(SyntaxKind.IdentifierToken, "a")
			};
	}
}