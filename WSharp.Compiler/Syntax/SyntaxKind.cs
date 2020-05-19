namespace WSharp.Compiler.Syntax
{
	public enum SyntaxKind
	{
		// Tokens
		EndOfFileToken,
		NumberToken,
		StringToken,
		PlusToken,
		MinusToken,
		StarToken,
		SlashToken,
		PercentToken,
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

		// Trivia
		BadTokenTrivia,
		WhitespaceTrivia,
		SingleLineCommentTrivia,
		MultiLineCommentTrivia,
		LineBreakTrivia,

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
		UnaryExpression,
		BinaryExpression,
		ParenthesizedExpression,
		UpdateLineCountExpression,
		UnaryUpdateLineCountExpression,
		CallExpression,
	}
}