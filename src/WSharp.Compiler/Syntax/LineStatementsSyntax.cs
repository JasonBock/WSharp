namespace WSharp.Compiler.Syntax;

public sealed class LineStatementsSyntax
	: StatementSyntax
{
	internal LineStatementsSyntax(SyntaxTree tree, List<LineStatementSyntax> lines)
		: base(tree) =>
			this.Lines = lines;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		foreach (var line in this.Lines)
		{
			yield return line;
		}
	}

#pragma warning disable CA1002 // Do not expose generic lists
	public List<LineStatementSyntax> Lines { get; }
#pragma warning restore CA1002 // Do not expose generic lists
	public override SyntaxKind Kind => SyntaxKind.LineStatements;
}