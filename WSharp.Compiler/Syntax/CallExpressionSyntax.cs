using System.Collections.Generic;

namespace WSharp.Compiler.Syntax
{
	public sealed class CallExpressionSyntax
		: ExpressionSyntax
	{
		internal CallExpressionSyntax(SyntaxTree tree, SyntaxToken identifier, SyntaxToken openParenthesisToken,
			SeparatedSyntaxList<ExpressionSyntax> arguments, SyntaxToken closeParenthesisToken)
			: base(tree) => 
				(this.Identifier, this.OpenParenthesisToken, this.Arguments, this.CloseParenthesisToken) =
					(identifier, openParenthesisToken, arguments, closeParenthesisToken);

		public SyntaxToken Identifier { get; }
		public override SyntaxKind Kind => SyntaxKind.CallExpression;
		public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
		public SyntaxToken OpenParenthesisToken { get; }
		public SyntaxToken CloseParenthesisToken { get; }

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.Identifier;
			yield return this.OpenParenthesisToken;

			foreach(var argument in this.Arguments.GetAllNodes())
			{
				yield return argument;
			}

			yield return this.CloseParenthesisToken;
		}
	}
}