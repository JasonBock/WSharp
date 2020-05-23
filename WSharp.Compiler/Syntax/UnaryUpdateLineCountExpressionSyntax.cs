using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class UnaryUpdateLineCountExpressionSyntax
		: ExpressionSyntax
	{
		internal UnaryUpdateLineCountExpressionSyntax(SyntaxTree tree, ExpressionSyntax lineNumber)
			: base(tree) =>
				this.LineNumber = lineNumber;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.LineNumber;
		}

		public override SyntaxKind Kind => SyntaxKind.UnaryUpdateLineCountExpression;
		public ExpressionSyntax LineNumber { get; }
	}
}