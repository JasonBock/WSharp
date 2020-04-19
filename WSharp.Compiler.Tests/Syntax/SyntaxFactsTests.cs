using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests.Compiler.Syntax
{
	public static class SyntaxFactsTests
	{
		[TestCaseSource(nameof(SyntaxFactsTests.GetSyntaxKindData))]
		public static void RoundTripSyntaxKind(SyntaxKind kind)
		{
			var text = SyntaxFacts.GetText(kind);

			if(!string.IsNullOrWhiteSpace(text))
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

		private static IEnumerable<SyntaxKind> GetSyntaxKindData()
		{
			foreach(var kind in (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind)))
			{
				yield return kind;
			}
		}
	}
}
