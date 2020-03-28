using System;
using System.Collections.Generic;
using System.Linq;
using Spackle;
using Spackle.Extensions;
using WSharp.Runtime;
using WSharp.Runtime.Compiler;
using WSharp.Runtime.Compiler.Binding;
using WSharp.Runtime.Compiler.Syntax;

// To be clear, what this project is meant for is to work on
// building the compiler for WSharp.Runtime. This is new territory for me,
// and I feel like just experimenting with ideas and learning.
// Once I have things more concrete in place, I'll add that all
// into WSharp.Runtime proper (or put it into its own assembly
// like WSharp.Compiler, we'll see).
namespace WSharp.Playground
{
	public static class Program
	{
		public const string Branch = "└──";
		public const string Down = "│  ";
		public const string Center = "├──";
		public const string Space = "   ";

		private static void Print(SyntaxNode node, string indent = "", bool isLast = true)
		{
			var marker = isLast ? Program.Branch : Program.Center;

			Console.Out.Write(indent);
			Console.Out.Write(marker);
			Console.Out.Write(node.Kind);

			if (node is SyntaxToken token && token.Value != null)
			{
				Console.Write($" {token.Value}");
			}

			Console.Out.WriteLine();

			indent += isLast ? Program.Space : Program.Down;

			var lastChild = node.GetChildren().LastOrDefault();

			foreach (var child in node.GetChildren())
			{
				Program.Print(child, indent, child == lastChild);
			}
		}

		public static void Main() =>
			Program.RunRepl();

		private static void RunEvaluator(BoundExpression expression)
		{
			var lines = BoundEvaluatorGenerator.Generate(new List<BoundExpression> { expression });
			var engine = new ExecutionEngine(lines, new SecureRandom(), Console.Out);
			Console.Out.WriteLine("ExecutionEngine ready...");
			//engine.Execute();
		}

		private static void RunRepl()
		{
			var showTree = true;

			while (true)
			{
				Console.Out.Write("> ");
				var line = Console.In.ReadLine();

				if (!string.IsNullOrWhiteSpace(line))
				{
					if(line == "#showTree")
					{
						showTree = !showTree;
						Console.Out.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
						continue;
					}
					else if(line == "#cls")
					{
						Console.Clear();
						continue;
					}

					var tree = SyntaxTree.Parse(line);
					var binder = new Binder();
					var boundExpression = binder.BindExpression(tree.Root);
					var diagnostics = tree.Diagnostics.Concat(binder.Diagnostics).ToArray();

					if(showTree)
					{
						using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
						{
							Program.Print(tree.Root);
						}
					}

					if (diagnostics.Length > 0)
					{
						using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
						{
							foreach (var diagnostic in diagnostics)
							{
								Console.Out.WriteLine(diagnostic);
							}
						}
					}
					else
					{
						Program.RunEvaluator(boundExpression);
					}
				}
				else
				{
					return;
				}
			}
		}
	}
}