using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax;

public abstract class SyntaxNode
{
	protected SyntaxNode(SyntaxTree tree) => this.Tree = tree;

	public IEnumerable<SyntaxNode> Ancestors() => this.AncestorsAndSelf().Skip(1);

	public IEnumerable<SyntaxNode> AncestorsAndSelf()
	{
		var node = this;

		while (node is { })
		{
			yield return node;
			node = node.Parent;
		}
	}

	// TODO: Can I use this.parents to descend?
	public IEnumerable<SyntaxNode> DescendentNodes()
	{
		static IEnumerable<SyntaxNode> Descend(SyntaxNode node)
		{
			yield return node;

			foreach (var childNode in node.GetChildren())
			{
				foreach (var descendNode in Descend(childNode))
				{
					yield return descendNode;
				}
			}
		}

		foreach (var childNode in Descend(this))
		{
			yield return childNode;
		}
	}

	public abstract IEnumerable<SyntaxNode> GetChildren();

	public SyntaxToken GetLastToken() =>
		this is SyntaxToken token ? token : this.GetChildren().Last().GetLastToken();

	private static void Print(TextWriter writer, SyntaxNode? node, string indent = "", bool isLast = true)
	{
		if (node is null)
		{
			return;
		}

		var token = node as SyntaxToken;

		if (token is { })
		{
			foreach (var trivia in token.LeadingTrivia)
			{
				writer.Write(indent);
				writer.Write(TreePrint.Center);
				writer.WriteLine($"L: {trivia.Kind}");
			}
		}

		var hasTrailingTrivia = token is { } && token.TrailingTrivia.Length > 0;
		var tokenMarker = !hasTrailingTrivia && isLast ? TreePrint.Branch : TreePrint.Center;

		writer.Write(indent);
		writer.Write(tokenMarker);
		writer.Write(node.Kind);

		if (token is { } && token.Value is { })
		{
			writer.Write(" ");
			writer.Write($" {token.Value}");
		}

		writer.WriteLine();

		if (token is { })
		{
			foreach (var trivia in token.TrailingTrivia)
			{
				var isLastTrailingTrivia = trivia == token.TrailingTrivia[^1];
				var triviaMarker = isLast && isLastTrailingTrivia ? TreePrint.Branch : TreePrint.Center;

				writer.Write(indent);
				writer.Write(triviaMarker);
				writer.WriteLine($"T: {trivia.Kind}");
			}
		}

		indent += isLast ? TreePrint.Space : TreePrint.Down;

		var lastChild = node.GetChildren().LastOrDefault();

		foreach (var child in node.GetChildren())
		{
			SyntaxNode.Print(writer, child, indent, child == lastChild);
		}
	}

	public override string ToString() => $"{this.Kind}, {this.Span}";

	public void WriteTo(TextWriter writer)
	{
		ArgumentNullException.ThrowIfNull(writer);
		SyntaxNode.Print(writer, this);
	}

	public virtual TextSpan FullSpan
	{
		get
		{
			var first = this.GetChildren().First().FullSpan;
			var last = this.GetChildren().Last().FullSpan;
			return TextSpan.FromBounds(first.Start, last.End);
		}
	}

	public abstract SyntaxKind Kind { get; }

	public TextLocation Location => new(this.Tree.Text, this.Span);

	public SyntaxNode? Parent => this.Tree.GetParent(this);

	public virtual TextSpan Span
	{
		get
		{
			var first = this.GetChildren().First().Span;
			var last = this.GetChildren().Last().Span;
			return TextSpan.FromBounds(first.Start, last.End);
		}
	}

	public SyntaxTree Tree { get; }
}