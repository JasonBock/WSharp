namespace WSharp.Compiler.Syntax;

public sealed class ParenthesizedExpressionSyntax
	: ExpressionSyntax
{
	internal ParenthesizedExpressionSyntax(SyntaxTree tree, SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
		: base(tree) =>
			(this.OpenParenthesisToken, this.Expression, this.CloseParenthesisToken) =
				(openParenthesisToken, expression, closeParenthesisToken);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return this.OpenParenthesisToken;
		yield return this.Expression;
		yield return this.CloseParenthesisToken;
	}

	public SyntaxToken CloseParenthesisToken { get; }
	public ExpressionSyntax Expression { get; }
	public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
	public SyntaxToken OpenParenthesisToken { get; }
}