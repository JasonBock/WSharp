using System;
using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public static class SyntaxFacts
	{
		public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
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
				case SyntaxKind.TildeToken:
					return 6;
				default:
					return 0;
			}
		}

		public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
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
				case SyntaxKind.PercentToken:
					return 5;
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
					return 4;
				case SyntaxKind.EqualsEqualsToken:
				case SyntaxKind.BangEqualsToken:
				case SyntaxKind.LessToken:
				case SyntaxKind.LessOrEqualsToken:
				case SyntaxKind.GreaterToken:
				case SyntaxKind.GreaterOrEqualsToken:
					return 3;
				case SyntaxKind.AmpersandToken:
				case SyntaxKind.AmpersandAmpersandToken:
					return 2;
				case SyntaxKind.PipeToken:
				case SyntaxKind.PipePipeToken:
				case SyntaxKind.HatToken:
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
				SyntaxKind.CloseParenthesisToken => ")",
				SyntaxKind.CommaToken => ",",
				SyntaxKind.BangEqualsToken => "!=",
				SyntaxKind.EqualsEqualsToken => "==",
				SyntaxKind.PipeToken => "|",
				SyntaxKind.PipePipeToken => "||",
				SyntaxKind.AmpersandToken => "&",
				SyntaxKind.AmpersandAmpersandToken => "&&",
				SyntaxKind.HatToken => "^",
				SyntaxKind.TildeToken => "~",
				SyntaxKind.BangToken => "!",
				SyntaxKind.LessToken => "<",
				SyntaxKind.LessOrEqualsToken => "<=",
				SyntaxKind.GreaterToken => ">",
				SyntaxKind.GreaterOrEqualsToken => ">=",
				SyntaxKind.SlashToken => "/",
				SyntaxKind.StarToken => "*",
				SyntaxKind.MinusToken => "-",
				SyntaxKind.PlusToken => "+",
				SyntaxKind.PercentToken => "%",
				SyntaxKind.UpdateLineCountToken => "#",
				SyntaxKind.SemicolonToken => ";",
				_ => string.Empty
			};

		public static bool IsComment(this SyntaxKind @this) =>
			@this == SyntaxKind.SingleLineCommentTrivia ||
			@this == SyntaxKind.MultiLineCommentTrivia;

		public static bool IsKeyword(this SyntaxKind @this) =>
			@this.ToString().EndsWith("Keyword");

		public static bool IsToken(this SyntaxKind @this) =>
			!@this.IsTrivia() && 
				(@this.IsKeyword() || @this.ToString().EndsWith("Token"));

		public static bool IsTrivia(this SyntaxKind @this) =>
			@this switch
			{
				SyntaxKind.SkippedTextTrivia => true,
				SyntaxKind.LineBreakTrivia => true,
				SyntaxKind.WhitespaceTrivia => true,
				SyntaxKind.SingleLineCommentTrivia => true,
				SyntaxKind.MultiLineCommentTrivia => true,
				SyntaxKind.BadToken => true,
				_ => false
			};
	}
}