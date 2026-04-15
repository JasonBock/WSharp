using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundExpressionStatement
	: BoundStatement
{
	public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression)
		: base(syntax) => this.Expression = expression;

	public override IEnumerable<BoundNode> GetChildren()
	{
		yield return this.Expression;
	}

	public override IEnumerable<(string name, object value)> GetProperties() =>
		[];

	public BoundExpression Expression { get; }
	public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
}