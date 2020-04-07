using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class UnaryUpdateLineCountExpressionSyntax
		: ExpressionSyntax
	{
		public UnaryUpdateLineCountExpressionSyntax(ExpressionSyntax lineNumber) =>
			this.LineNumber = lineNumber;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.LineNumber;
		}

		public override SyntaxKind Kind => SyntaxKind.UnaryUpdateLineCountExpression;
		public ExpressionSyntax LineNumber { get; }
	}
}
