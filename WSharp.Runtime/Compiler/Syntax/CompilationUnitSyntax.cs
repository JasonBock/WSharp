using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class CompilationUnitSyntax
		: SyntaxNode
	{
		public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken) => 
			(this.Statement, this.EndOfFileToken) = (statement, endOfFileToken);

		public SyntaxToken EndOfFileToken { get; }
		public StatementSyntax Statement { get; }

		public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Statement;
			yield return this.EndOfFileToken;
		}
	}
}