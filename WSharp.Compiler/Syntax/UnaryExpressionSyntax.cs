using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class UnaryExpressionSyntax
		: ExpressionSyntax
	{
		public UnaryExpressionSyntax(SyntaxTree tree, SyntaxToken operatorToken, ExpressionSyntax operand)
			: base(tree) => 
				(this.OperatorToken, this.Operand) = (operatorToken, operand);

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.OperatorToken;
			yield return this.Operand;
		}

		public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Operand { get; }
	}
}