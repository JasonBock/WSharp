namespace WSharp.Compiler.Syntax;

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

	public static int GetUnaryOperatorPrecedence(this SyntaxKind self) =>
		self switch
		{
			SyntaxKind.PlusToken or SyntaxKind.MinusToken or
				SyntaxKind.BangToken or SyntaxKind.TildeToken => 6,
			_ => 0,
		};

	public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
	{
		foreach (var kind in (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind)))
		{
			if (kind.GetBinaryOperatorPrecedence() > 0)
			{
				yield return kind;
			}
		}
	}

	public static int GetBinaryOperatorPrecedence(this SyntaxKind self) =>
		self switch
		{
			SyntaxKind.StarToken or SyntaxKind.SlashToken or SyntaxKind.PercentToken => 5,
			SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,
			SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken or SyntaxKind.LessToken or
				SyntaxKind.LessOrEqualsToken or SyntaxKind.GreaterToken or SyntaxKind.GreaterOrEqualsToken => 3,
			SyntaxKind.AmpersandToken or SyntaxKind.AmpersandAmpersandToken => 2,
			SyntaxKind.PipeToken or SyntaxKind.PipePipeToken or SyntaxKind.HatToken => 1,
			_ => 0,
		};

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

	public static bool IsComment(this SyntaxKind self) =>
		self == SyntaxKind.SingleLineCommentTrivia ||
		self == SyntaxKind.MultiLineCommentTrivia;

	public static bool IsKeyword(this SyntaxKind self) =>
		self.ToString().EndsWith("Keyword", StringComparison.InvariantCulture);

	public static bool IsToken(this SyntaxKind self) =>
		!self.IsTrivia() &&
			(self.IsKeyword() || self.ToString().EndsWith("Token", StringComparison.InvariantCulture));

	public static bool IsTrivia(this SyntaxKind self) =>
		self switch
		{
			SyntaxKind.SkippedTextTrivia or SyntaxKind.LineBreakTrivia or
				SyntaxKind.WhitespaceTrivia or SyntaxKind.SingleLineCommentTrivia or
				SyntaxKind.MultiLineCommentTrivia or SyntaxKind.BadToken => true,
			_ => false
		};
}