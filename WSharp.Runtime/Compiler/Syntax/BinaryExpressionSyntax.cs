using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class BinaryExpressionSyntax 
		: ExpressionSyntax
	{
		public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) => 
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