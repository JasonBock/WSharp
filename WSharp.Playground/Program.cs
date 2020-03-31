using System;
using System.Linq;
using Spackle;
using Spackle.Extensions;
using WSharp.Runtime;
using WSharp.Runtime.Compiler;
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
		public static void Main() =>
			Program.RunRepl();

		private static void RunEvaluator(EvaluationResult evaluation)
		{
			var engine = new ExecutionEngine(evaluation.Lines, new SecureRandom(), Console.Out);
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
					if (line == "#showTree")
					{
						showTree = !showTree;
						Console.Out.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
						continue;
					}
					else if (line == "#cls")
					{
						Console.Clear();
						continue;
					}

					var tree = SyntaxTree.Parse(line);
					var binder = new Compilation(tree);
					var result = binder.Evaluate();
					var diagnostics = result.Diagnostics;

					if (showTree)
					{
						using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
						{
							tree.Root.WriteTo(Console.Out);
						}
					}

					if (diagnostics.Any())
					{
						var text = tree.Text;

						foreach (var diagnostic in diagnostics)
						{
							var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
							var lineNumber = lineIndex + 1;
							var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;

							Console.Out.WriteLine();
							using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
							{
								Console.Out.WriteLine($"({lineNumber}, {character}): {diagnostic}");
							}

							var prefix = line.Substring(0, diagnostic.Span.Start);
							var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
							var suffix = line.Substring(diagnostic.Span.End);

							Console.Out.Write(SyntaxNode.Space);
							Console.Out.Write(prefix);

							using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
							{
								Console.Out.Write(error);
							}

							Console.Out.Write(suffix);
							Console.Out.WriteLine();
						}

						Console.Out.WriteLine();
					}
					else
					{
						Program.RunEvaluator(result);
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