namespace WSharp.Compiler.Syntax;

public sealed class LiteralExpressionSyntax
	: ExpressionSyntax
{
	internal LiteralExpressionSyntax(SyntaxTree tree, SyntaxToken literalToken)
		: this(tree, literalToken, literalToken.Value!) { }

	internal LiteralExpressionSyntax(SyntaxTree tree, SyntaxToken literalToken, object value)
		: base(tree) => (this.LiteralToken, this.Value) = (literalToken, value);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return this.LiteralToken;
	}

	public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
	public SyntaxToken LiteralToken { get; }
	public object Value { get; }
}