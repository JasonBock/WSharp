using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class LineStatementsSyntax
		: StatementSyntax
	{
		public LineStatementsSyntax(List<LineStatementSyntax> lines) =>
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