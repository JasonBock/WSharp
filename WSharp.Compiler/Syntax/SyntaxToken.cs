using System.Collections.Generic;
using System.Linq;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax
{
	public sealed class SyntaxToken 
		: SyntaxNode
	{
		public SyntaxToken(SyntaxTree tree, SyntaxKind kind, int position, string text, object? value)
			: base(tree) =>
				(this.Kind, this.Position, this.Text, this.Value) = 
					(kind, position, text, value);

		public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

		public bool IsMissing => this.Text is null;
		public override SyntaxKind Kind { get; }
		public int Position { get; }
		public override TextSpan Span => new TextSpan(this.Position, this.Text.Length);
		public string Text { get; }
		public object? Value { get; }
	}
}