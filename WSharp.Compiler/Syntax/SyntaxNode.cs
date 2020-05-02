﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax
{
	public abstract class SyntaxNode
	{
		protected SyntaxNode(SyntaxTree tree) => this.Tree = tree;

		public abstract IEnumerable<SyntaxNode> GetChildren();

		public void WriteTo(TextWriter writer)
		{
			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			SyntaxNode.Print(writer, this);
		}

		public TextLocation Location => new TextLocation(this.Tree.Text, this.Span);

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

		public override string ToString() => $"{this.Kind}, {this.Span}";

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

		public SyntaxTree Tree { get; }
	}
}