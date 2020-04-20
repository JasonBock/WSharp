using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class LineStatementsSyntax
		: StatementSyntax
	{
		public LineStatementsSyntax(SyntaxTree tree, List<LineStatementSyntax> lines)
			: base(tree) =>
				this.Lines = lines;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			foreach (var line in this.Lines)
			{
				yield return line;
			}
		}

		public List<LineStatementSyntax> Lines { get; }
		public override SyntaxKind Kind => SyntaxKind.LineStatements;
	}
}