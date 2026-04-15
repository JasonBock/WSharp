namespace WSharp.Compiler.Syntax;

public sealed class LineStatementSyntax
	: StatementSyntax
{
	internal LineStatementSyntax(SyntaxTree tree, ExpressionStatementSyntax number, List<ExpressionStatementSyntax> statements)
		: base(tree) =>
			(this.Number, this.Statements) = (number, statements);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return this.Number;

		foreach (var expression in this.Statements)
		{
			yield return expression;
		}
	}

#pragma warning disable CA1002 // Do not expose generic lists
	public List<ExpressionStatementSyntax> Statements { get; }
#pragma warning restore CA1002 // Do not expose generic lists
	public override SyntaxKind Kind => SyntaxKind.LineStatement;
	public ExpressionStatementSyntax Number { get; }
}