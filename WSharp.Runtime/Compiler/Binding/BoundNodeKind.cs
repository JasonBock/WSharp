namespace WSharp.Runtime.Compiler.Binding
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
	}
}