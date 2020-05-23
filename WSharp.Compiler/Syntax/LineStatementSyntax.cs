using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class LineStatementSyntax 
		: StatementSyntax
	{
		internal LineStatementSyntax(SyntaxTree tree, ExpressionStatementSyntax number, List<ExpressionStatementSyntax> statements) 
			: base(tree) =>
				(this.Number, this.Statements) = (number, statements);

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Number;

			foreach(var expression in this.Statements)
			{
				yield return expression;
			}
		}

		public List<ExpressionStatementSyntax> Statements { get; }
		public override SyntaxKind Kind => SyntaxKind.LineStatement;
		public ExpressionStatementSyntax Number { get; }
	}
}