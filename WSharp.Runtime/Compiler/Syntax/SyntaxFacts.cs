using System;
using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public static class SyntaxFacts
	{
		public static IEnumerable<SyntaxKind> GetUnaryOperators()
		{
			foreach (var kind in (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind)))
			{
				if (kind.GetUnaryOperatorPrecedence() > 0)
				{
					yield return kind;
				}
			}
		}

		public static int GetUnaryOperatorPrecedence(this SyntaxKind @this)
		{
			switch (@this)
			{
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
				case SyntaxKind.BangToken:
					return 6;
				default:
					return 0;
			}
		}

		public static IEnumerable<SyntaxKind> GetBinaryOperators()
		{
			foreach (var kind in (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind)))
			{
				if(kind.GetBinaryOperatorPrecedence() > 0)
				{
					yield return kind;
				}
			}
		}

		public static int GetBinaryOperatorPrecedence(this SyntaxKind @this)
		{
			switch (@this)
			{
				case SyntaxKind.StarToken:
				case SyntaxKind.SlashToken:
					return 5;
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
					return 4;
				case SyntaxKind.EqualsEqualsToken:
				case SyntaxKind.BangEqualsToken:
					return 3;
				case SyntaxKind.AmpersandAmpersandToken:
					return 2;
				case SyntaxKind.PipePipeToken:
					return 1;
				default:
					return 0;
			}
		}

		public static SyntaxKind GetKeywordKind(string text) =>
			text switch
			{
				"true" => SyntaxKind.TrueKeyword,
				"false" => SyntaxKind.FalseKeyword,
				_ => SyntaxKind.IdentifierToken,
			};

		public static string GetText(SyntaxKind kind) =>
			kind switch
			{
				SyntaxKind.FalseKeyword => "false",
				SyntaxKind.TrueKeyword => "true",
				SyntaxKind.OpenParenthesisToken => "(",
				SyntaxKind.BangEqualsToken => "!=",
				SyntaxKind.EqualsEqualsToken => "==",
				SyntaxKind.PipePipeToken => "||",
				SyntaxKind.AmpersandAmpersandToken => "&&",
				SyntaxKind.BangToken => "!",
				SyntaxKind.SlashToken => "/",
				SyntaxKind.StarToken => "*",
				SyntaxKind.MinusToken => "-",
				SyntaxKind.PlusToken => "+",
				_ => string.Empty
			};
	}
}