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
		LessOrEqualsToken,
		LessToken,
		GreaterOrEqualsToken,
		GreaterToken,
		TildeToken,
		AmpersandToken,
		AmpersandAmpersandToken,
		PipeToken,
		PipePipeToken,
		HatToken,
		BangEqualsToken,
		EqualsEqualsToken,
		OpenParenthesisToken,
		CloseParenthesisToken,
		UpdateLineCountToken,
		CommaToken,
		SemicolonToken,
		IdentifierToken,

		// Keywords
		FalseKeyword,
		TrueKeyword,

		// Nodes
		CompilationUnit,

		// Statements
		LineStatements,
		LineStatement,
		ExpressionStatement,

		// Expressions
		LiteralExpression,
		NameExpression,
		UnaryExpression,
		BinaryExpression,
		ParenthesizedExpression,
		UpdateLineCountExpression,
		UnaryUpdateLineCountExpression,
	}
}