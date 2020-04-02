﻿namespace WSharp.Runtime.Compiler.Syntax
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
		AmpersandAmpersandToken,
		PipePipeToken,
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
		LineStatement,
		ExpressionStatement,

		// Expressions
		LiteralExpression,
		NameExpression,
		UnaryExpression,
		BinaryExpression,
		ParenthesizedExpression,
		UpdateLineCountExpression,
	}
}