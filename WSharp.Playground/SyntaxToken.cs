using System.Collections.Generic;
using System.Linq;

namespace WSharp.Playground
{
	public sealed class SyntaxToken : SyntaxNode
	{
		private SyntaxToken() =>
			(this.Text, this.Value) = (string.Empty, null);

		public SyntaxToken(SyntaxKind kind, int position, string text, object? value) =>
			(this.Kind, this.Position, this.Text, this.Value) = (kind, position, text, value);

		public override string ToString() => $"{this.Kind} - {this.Value}";

		public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

		public override SyntaxKind Kind { get; }
		public int Position { get; }
		public string Text { get; }
		public object? Value { get; }
	}
}