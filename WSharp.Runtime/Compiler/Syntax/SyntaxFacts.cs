using System;

namespace WSharp.Runtime.Compiler.Syntax
{
	public static class SyntaxFacts
	{
		public static int GetUnaryOperatorPrecedence(this SyntaxKind @this)
		{
			switch (@this)
			{
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
				case SyntaxKind.BangToken:
					return 5;
				default:
					return 0;
			}
		}

		public static int GetBinaryOperatorPrecedence(this SyntaxKind @this)
		{
			switch (@this)
			{
				case SyntaxKind.StarToken:
				case SyntaxKind.SlashToken:
					return 4;
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
					return 3;
				case SyntaxKind.AmpersandAmpersandToken:
					return 2;
				case SyntaxKind.PipePipeToken:
					return 1;
				default:
					return 0;
			}
		}

		public static SyntaxKind GetKeywordKind(string text)
		{
			switch(text)
			{
				case "true":
					return SyntaxKind.TrueKeyword;
				case "false":
					return SyntaxKind.FalseKeyword;
				default:
					return SyntaxKind.IdentifierToken;
			}
		}
	}
}
