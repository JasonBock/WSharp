using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax;

public sealed class SyntaxTrivia
{
	public SyntaxTrivia(SyntaxTree tree, SyntaxKind kind, int position, string text) =>
		(this.Tree, this.Kind, this.Position, this.Text) = (tree, kind, position, text);

	public SyntaxKind Kind { get; }
	public int Position { get; }
	public TextSpan Span => new(this.Position, this.Text?.Length ?? 0);
	public string Text { get; }
	public SyntaxTree Tree { get; }
}