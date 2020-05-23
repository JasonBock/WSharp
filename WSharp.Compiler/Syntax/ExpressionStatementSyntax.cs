using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class ExpressionStatementSyntax
		: StatementSyntax
	{
		internal ExpressionStatementSyntax(SyntaxTree tree, ExpressionSyntax expression)
			: base(tree) => 
				this.Expression = expression;

		public ExpressionSyntax Expression { get; }

		public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Expression;
		}
	}
}