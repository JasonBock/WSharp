﻿using System.Collections.Generic;
using System.Linq;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class SyntaxToken 
		: SyntaxNode
	{
		private SyntaxToken() =>
			(this.Text, this.Value) = (string.Empty, null);

		public SyntaxToken(SyntaxKind kind, int position, string text, object? value) =>
			(this.Kind, this.Position, this.Text, this.Value) = (kind, position, text, value);

		public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

		public override SyntaxKind Kind { get; }
		public int Position { get; }
		public TextSpan Span => new TextSpan(this.Position, this.Text.Length);
		public string Text { get; }
		public object? Value { get; }
	}
}