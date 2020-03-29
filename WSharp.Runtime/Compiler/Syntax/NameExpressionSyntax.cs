using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class NameExpressionSyntax
		: ExpressionSyntax
	{
		public NameExpressionSyntax(SyntaxToken identifierToken) => 
			this.IdentifierToken = identifierToken;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.IdentifierToken;
		}

		public SyntaxToken IdentifierToken { get; }
		public override SyntaxKind Kind => SyntaxKind.NameExpression;
	}
}