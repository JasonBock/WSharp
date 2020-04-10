using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WSharp.Runtime.Compiler.Binding
{
	public abstract class BoundNode
	{
		public abstract IEnumerable<BoundNode> GetChildren();

		// TODO: At some point, Minsk removes this from BoundNode. If it completely goes away, OK,
		// but if it moves somewhere else, I think this one may be OK to implement
		// with Reflection, so long as it doesn't rely on member ordering in the file.
		public abstract IEnumerable<(string name, object value)> GetProperties();

		public void WriteTo(TextWriter writer) =>
			BoundNode.Print(writer, this);

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

		private static void WriteProperties(TextWriter writer, BoundNode node) => 
			writer.Write($"{string.Join(",", node.GetProperties().Select(_ => $" {_.name} = {_.value}"))}");

		private static void WriteNode(TextWriter writer, BoundNode node) => 
			writer.Write(BoundNode.GetText(node));

		private static string GetText(BoundNode node) =>
			node switch
			{
				BoundBinaryExpression binary => $"{binary.Operator.OperatorKind}Expression",
				BoundUnaryExpression unary => $"{unary.Operator.OperatorKind}Expression",
				_ => node.Kind.ToString()
			};

		public abstract BoundNodeKind Kind { get; }
	}
}