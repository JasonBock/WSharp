namespace WSharp.Compiler.Binding
{
	public enum BoundNodeKind
	{
		// Statements
		LineStatements,
		LineStatement,
		ExpressionStatement,

		// Expressions
		UnaryExpression,
		LiteralExpression,
		BinaryExpression,
		UpdateLineCountExpression,
		UnaryUpdateLineCountExpression,
		CallExpression,
		ErrorExpression,
		ConversionExpression,
	}
}