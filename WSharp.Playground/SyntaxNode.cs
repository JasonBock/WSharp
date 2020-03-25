using System.Collections.Generic;

namespace WSharp.Playground
{
	// 1 2#3,3#4
	/*
	      1
        / \
       #   #
	   / \ / \ 
	  2  3 3  4
	*/
	public abstract class SyntaxNode
	{
		public abstract IEnumerable<SyntaxNode> GetChildren();

		public abstract SyntaxKind Kind { get; }
	}

	// LineNumberNode
	// NumberNode
	// UpdateLineNumberNode

	public abstract class ExpressionSyntax : SyntaxNode { }

	public sealed class NumberExpressionSyntax : ExpressionSyntax
	{
		public NumberExpressionSyntax(SyntaxToken numberToken) =>
			this.NumberToken = numberToken;

		public override SyntaxKind Kind => SyntaxKind.NumberExpression;

		public SyntaxToken NumberToken { get; }

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.NumberToken;
		}
	}

	public sealed class LineExpressionSyntax : ExpressionSyntax
	{
		public LineExpressionSyntax(ExpressionSyntax number, List<ExpressionSyntax> expressions) =>
			(this.Number, this.Expressions) = (number, expressions);

		public override SyntaxKind Kind => SyntaxKind.LineExpression;

		public ExpressionSyntax Number { get; }
		public List<ExpressionSyntax> Expressions { get; }

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Number;

			foreach(var expression in this.Expressions)
			{
				yield return expression;
			}
		}
	}

	public sealed class UpdateLineCountExpressionSyntax : ExpressionSyntax
	{
		public UpdateLineCountExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) =>
			(this.Left, this.OperatorToken, this.Right) = (left, operatorToken, right);

		public override SyntaxKind Kind => SyntaxKind.UpdateLineCountExpression;

		public ExpressionSyntax Left { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Right { get; }

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Left;
			yield return this.OperatorToken;
			yield return this.Right;
		}
	}

	public sealed class BinaryExpressionSyntax : ExpressionSyntax
	{
		public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) => 
			(this.Left, this.OperatorToken, this.Right) = (left, operatorToken, right);

		public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

		public ExpressionSyntax Left { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Right { get; }

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Left;
			yield return this.OperatorToken;
			yield return this.Right;
		}
	}
}