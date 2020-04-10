using System.Collections.Generic;
using System.IO;
using System.Linq;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler.Syntax
{
	// TODO: The ToString() can get very heavy to see in a debugger.
	// Consider adding DebuggerDiagnostic attribute here to have a concise
	// view of the node. Maybe something like "{this.Kind}, {this.Span}"
	public abstract class SyntaxNode
	{
		public abstract IEnumerable<SyntaxNode> GetChildren();

		public void WriteTo(TextWriter writer) => 
			SyntaxNode.Print(writer, this);

		public SyntaxToken GetLastToken()
		{
			if (this is SyntaxToken token)
				return token;

			return this.GetChildren().Last().GetLastToken();
		}

		private static void Print(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
		{
			var marker = isLast ? TreePrint.Branch : TreePrint.Center;

			writer.Write(indent);
			writer.Write(marker);
			writer.Write(node.Kind);

			if (node is SyntaxToken token && token.Value != null)
			{
				writer.Write($" {token.Value}");
			}

			writer.WriteLine();

			indent += isLast ? TreePrint.Space : TreePrint.Down;

			var lastChild = node.GetChildren().LastOrDefault();

			foreach (var child in node.GetChildren())
			{
				SyntaxNode.Print(writer, child, indent, child == lastChild);
			}
		}

		public override string ToString()
		{
			using var writer = new StringWriter();
			this.WriteTo(writer);
			return writer.ToString();
		}

		public abstract SyntaxKind Kind { get; }

		public virtual TextSpan Span
		{
			get
			{
				var first = this.GetChildren().First().Span;
				var last = this.GetChildren().Last().Span;
				return TextSpan.FromBounds(first.Start, last.End);
			}
		}
	}
}