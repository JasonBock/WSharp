using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class LineExpressionSyntax 
		: ExpressionSyntax
	{
		public LineExpressionSyntax(LiteralExpressionSyntax number, List<ExpressionSyntax> expressions) =>
			(this.Number, this.Expressions) = (number, expressions);

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Number;

			foreach(var expression in this.Expressions)
			{
				yield return expression;
			}
		}

		public List<ExpressionSyntax> Expressions { get; }
		public override SyntaxKind Kind => SyntaxKind.LineExpression;
		public LiteralExpressionSyntax Number { get; }
	}
}