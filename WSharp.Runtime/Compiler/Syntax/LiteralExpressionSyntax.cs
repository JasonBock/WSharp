﻿using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class LiteralExpressionSyntax 
		: ExpressionSyntax
	{
		public LiteralExpressionSyntax(SyntaxToken literalToken) =>
			this.LiteralToken = literalToken;

		public override IEnumerable<SyntaxNode> GetChildren()
		{
			yield return this.LiteralToken;
		}

		public override SyntaxKind Kind => SyntaxKind.LiteralExpression ;
		public SyntaxToken LiteralToken { get; }
	}
}