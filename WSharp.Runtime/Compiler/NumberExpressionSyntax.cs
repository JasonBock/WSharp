using System.Collections.Generic;

namespace WSharp.Runtime.Compiler
{
	public sealed class NumberExpressionSyntax 
		: ExpressionSyntax
	{
		public NumberExpressionSyntax(SyntaxToken numberToken) =>
			this.NumberToken = numberToken;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.NumberToken;
		}

		public override SyntaxKind Kind => SyntaxKind.NumberExpression;
		public SyntaxToken NumberToken { get; }
	}
}