using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class CompilationUnitSyntax
		: SyntaxNode
	{
		public CompilationUnitSyntax(LineStatementsSyntax lineStatements, SyntaxToken endOfFileToken) => 
			(this.LineStatements, this.EndOfFileToken) = (lineStatements, endOfFileToken);

		public SyntaxToken EndOfFileToken { get; }
		public LineStatementsSyntax LineStatements { get; }

		public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			foreach(var line in this.LineStatements.Lines)
			{
				yield return line;
			}

			yield return this.EndOfFileToken;
		}
	}
}