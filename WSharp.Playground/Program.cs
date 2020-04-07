using System;
using System.Collections.Generic;
using System.Linq;
using Spackle;
using Spackle.Extensions;
using WSharp.Runtime;
using WSharp.Runtime.Compiler;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Playground
{
	internal sealed class WheneverRepl
		: Repl
	{ }

	internal class Repl
	{
		public void Run()
		{
			var showProgram = true;
			var showTree = true;
			var lines = new List<string>();

			while (true)
			{
				using (ConsoleColor.Green.Bind(() => Console.ForegroundColor))
				{
					Console.Out.Write("» ");
				}
				var line = Console.In.ReadLine();

				if (!string.IsNullOrWhiteSpace(line))
				{
					if (line == "#showTree")
					{
						showTree = !showTree;
						Console.Out.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
						continue;
					}
					else if (line == "#showProgram")
					{
						showProgram = !showProgram;
						Console.Out.WriteLine(showProgram ? "Showing program." : "Not showing program.");
						continue;
					}
					else if (line == "#clear")
					{
						Console.Clear();
						lines.Clear();
						continue;
					}
					else if(line == "#run")
					{
						Repl.Evaluate(showProgram, showTree, lines);
					}
					// TODO: Add #loadFile which has an argument of a file path, that loads code from the file.
					else
					{
						lines.Add(line);
					}
				}
				else
				{
					return;
				}
			}
		}

		private static void Evaluate(bool showProgram, bool showTree, List<string> lines)
		{
			// TODO: Add a command, #run, that would actually run the evaluator
			// This may require a change such that a "complete" submission is when I type #run,
			// because I may want to enter in multiple lines before doing an evaluation.
			var tree = SyntaxTree.Parse(string.Join(Environment.NewLine, lines));
			var compilation = new Compilation(tree);
			var result = compilation.Evaluate();
			var diagnostics = result.Diagnostics;

			if (showTree)
			{
				Console.Out.WriteLine("Tree:");
				using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
				{
					tree.Root.WriteTo(Console.Out);
				}
			}

			if (showProgram)
			{
				Console.Out.WriteLine("Program:");
				using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
				{
					compilation.EmitTree(Console.Out);
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

					var textLine = text.Lines[text.GetLineIndex(diagnostic.Span.Start)];
					var prefix = text.ToString(textLine.SpanIncludingLineBreak.Start, diagnostic.Span.Start - textLine.SpanIncludingLineBreak.Start);
					var error = text.ToString(diagnostic.Span.Start, diagnostic.Span.Length);
					var suffix = text.ToString(diagnostic.Span.End, textLine.SpanIncludingLineBreak.End - diagnostic.Span.End);

					Console.Out.Write(TreePrint.Space);
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
				Repl.RunEvaluator(result);
			}
		}

		private static void RunEvaluator(EvaluationResult evaluation)
		{
			var engine = new ExecutionEngine(evaluation.Lines, new SecureRandom(), Console.Out);
			Console.Out.WriteLine("ExecutionEngine ready...");
			engine.Execute();
			Console.Out.WriteLine("ExecutionEngine finished.");
		}
	}

	public static class Program
	{
		public static void Main() =>
			new Repl().Run();
	}
}