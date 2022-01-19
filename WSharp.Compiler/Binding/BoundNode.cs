using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal abstract class BoundNode
{
	protected BoundNode(SyntaxNode syntax) => this.Syntax = syntax;

	public abstract IEnumerable<BoundNode> GetChildren();

	public abstract IEnumerable<(string name, object value)> GetProperties();

	private static string GetText(BoundNode node) =>
		node switch
		{
			BoundBinaryExpression binary => $"{binary.Operator.OperatorKind}Expression",
			BoundUnaryExpression unary => $"{unary.Operator.OperatorKind}Expression",
			_ => node.Kind.ToString()
		};

	private static void Print(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
	{
		var marker = isLast ? TreePrint.Branch : TreePrint.Center;

		writer.Write(indent);
		writer.Write(marker);
		BoundNode.WriteNode(writer, node);
		BoundNode.WriteProperties(writer, node);
		writer.WriteLine();

		indent += isLast ? TreePrint.Space : TreePrint.Down;

		var lastChild = node.GetChildren().LastOrDefault();

		foreach (var child in node.GetChildren())
		{
			BoundNode.Print(writer, child, indent, child == lastChild);
		}
	}

	private static void WriteNode(TextWriter writer, BoundNode node) =>
		writer.Write(BoundNode.GetText(node));

	private static void WriteProperties(TextWriter writer, BoundNode node) =>
		writer.Write($"{string.Join(",", node.GetProperties().Select(_ => $" {_.name} = {_.value}"))}");

	public void WriteTo(TextWriter writer)
	{
		if (writer is null)
		{
			throw new ArgumentNullException(nameof(writer));
		}

		BoundNode.Print(writer, this);
	}

	public abstract BoundNodeKind Kind { get; }
	public SyntaxNode Syntax { get; }
}