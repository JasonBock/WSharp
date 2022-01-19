namespace WSharp.Compiler.Syntax;

public sealed class CompilationUnitSyntax
	: SyntaxNode
{
	internal CompilationUnitSyntax(SyntaxTree tree, LineStatementsSyntax lineStatements, SyntaxToken endOfFileToken)
		: base(tree) =>
			(this.LineStatements, this.EndOfFileToken) = (lineStatements, endOfFileToken);

	public SyntaxToken EndOfFileToken { get; }
	public LineStatementsSyntax LineStatements { get; }

	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return this.LineStatements;

		foreach (var line in this.LineStatements.Lines)
		{
			yield return line;
		}

		yield return this.EndOfFileToken;
	}
}