namespace WSharp.Runtime.Compiler.Binding
{
	public enum BoundNodeKind
	{
		// Statements
		ExpressionStatement,
		LineStatement,

		// Expressions
		UnaryExpression,
		LiteralExpression,
		BinaryExpression,
		UpdateLineCountExpression,
	}
}