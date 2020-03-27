namespace WSharp.Runtime.Compiler
{
	public enum SyntaxKind
	{
		NumberToken,
		WhitespaceToken,
		UpdateLineCountToken,
		BadToken,
		EndOfFileToken,
		CommaToken,
		NumberExpression,
		PlusToken,
		MinusToken,
		StarToken,
		SlashToken,
		OpenParenthesisToken,
		CloseParenthesisToken,
		BinaryExpression,
		UpdateLineCountExpression,
		LineExpression,
		ParenthesizedExpression,
		SemicolonToken
	}
}