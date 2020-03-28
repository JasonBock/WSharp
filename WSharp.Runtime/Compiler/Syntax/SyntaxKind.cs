namespace WSharp.Runtime.Compiler.Syntax
{
	public enum SyntaxKind
	{
		// Tokens
		BadToken,
		EndOfFileToken,
		WhitespaceToken,
		NumberToken,
		PlusToken,
		MinusToken,
		StarToken,
		SlashToken,
		BangToken,
		AmpersandAmpersandToken,
		PipePipeToken,
		OpenParenthesisToken,
		CloseParenthesisToken,
		UpdateLineCountToken,
		CommaToken,
		SemicolonToken,
		IdentifierToken,

		// Keywords
		FalseKeyword,
		TrueKeyword,

		// Expressions
		LiteralExpression,
		UnaryExpression,
		BinaryExpression,
		ParenthesizedExpression,
		UpdateLineCountExpression,
		LineExpression,
	}
}