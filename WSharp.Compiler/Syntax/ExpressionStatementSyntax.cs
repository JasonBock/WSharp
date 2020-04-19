using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class ExpressionStatementSyntax
		: StatementSyntax
	{
		public ExpressionStatementSyntax(ExpressionSyntax expression) => 
			this.Expression = expression;

		public ExpressionSyntax Expression { get; }

		public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Expression;
		}
	}
}
