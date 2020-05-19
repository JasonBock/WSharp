using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax
{
	public sealed class SyntaxToken 
		: SyntaxNode
	{
		public SyntaxToken(SyntaxTree tree, SyntaxKind kind, int position, string text, object? value, 
			ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia)
			: base(tree) =>
				(this.Kind, this.Position, this.Text, this.Value, this.LeadingTrivia, this.TrailingTrivia) = 
					(kind, position, text, value, leadingTrivia, trailingTrivia);

		public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

		public override TextSpan FullSpan
		{
			get
			{
				var start = this.LeadingTrivia.Length == 0 ? this.Span.Start : this.LeadingTrivia[0].Span.Start;
				var end = this.TrailingTrivia.Length == 0 ? this.Span.End : this.TrailingTrivia[^1].Span.End;
				return TextSpan.FromBounds(start, end); 
			}
		}

		public bool IsMissing => this.Text is null;
		public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }
		public override SyntaxKind Kind { get; }
		public int Position { get; }
		public override TextSpan Span => new TextSpan(this.Position, this.Text.Length);
		public string Text { get; }
		public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }
		public object? Value { get; }
	}
}