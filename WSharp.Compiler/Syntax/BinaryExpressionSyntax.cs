using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class BinaryExpressionSyntax 
		: ExpressionSyntax
	{
		internal BinaryExpressionSyntax(SyntaxTree tree, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
			: base(tree) => 
				(this.Left, this.OperatorToken, this.Right) = (left, operatorToken, right);

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Left;
			yield return this.OperatorToken;
			yield return this.Right;
		}

		public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
		public ExpressionSyntax Left { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Right { get; }
	}
}