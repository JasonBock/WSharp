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

		private static void Print(TextWriter writer, SyntaxNode? node, string indent = "", bool isLast = true)
		{
			if (node is null)
			{
				return;
			}

			var token = node as SyntaxToken;

			if(token is { })
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

		public abstract SyntaxKind Kind { get; }

		public virtual TextSpan FullSpan
		{
			get
			{
				var first = this.GetChildren().First().FullSpan;
				var last = this.GetChildren().Last().FullSpan;
				return TextSpan.FromBounds(first.Start, last.End);
			}
		}

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