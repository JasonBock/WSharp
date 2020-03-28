using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class LiteralExpressionSyntax 
		: ExpressionSyntax
	{
		public LiteralExpressionSyntax(SyntaxToken literalToken)
			: this(literalToken, literalToken.Value) { }

		public LiteralExpressionSyntax(SyntaxToken literalToken, object? value) =>
			(this.LiteralToken, this.Value) = (literalToken, value);

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.LiteralToken;
		}

		public override SyntaxKind Kind => SyntaxKind.LiteralExpression ;
		public SyntaxToken LiteralToken { get; }
		public object? Value { get; }
	}
}