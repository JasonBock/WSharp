namespace WSharp.Compiler.Binding;

public enum BoundNodeKind
{
	// Statements
	LineStatements,
	LineStatement,
	ExpressionStatement,
	SequencePointStatement,

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