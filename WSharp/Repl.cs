using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Spackle;
using Spackle.Extensions;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;
using WSharp.Runtime;

namespace WSharp
{
	internal sealed class Repl
	{
		private readonly List<string> lines = new List<string>();
		private bool showProgram = false;
		private bool showTree = false;

		public async Task RunAsync(FileInfo file)
		{
			await this.EvaluateMetaCommandAsync($"#loadFile {file.FullName}");
			await this.RunAsync();
		}

		public async Task RunAsync()
		{
			while (true)
			{
				using (ConsoleColor.Green.Bind(() => Console.ForegroundColor))
				{
					Console.Out.Write("» ");
				}

				var input = Console.In.ReadLine();

				if (!string.IsNullOrWhiteSpace(input))
				{
					if (input.StartsWith("#"))
					{
						await this.EvaluateMetaCommandAsync(input);
						continue;
					}
					else
					{
						this.lines.Add(input);
					}
				}
				else
				{
					return;
				}
			}
		}

		private async Task EvaluateMetaCommandAsync(string input)
		{
			if (input.StartsWith("#loadFile"))
			{
				var fileName = input.Substring("#loadFile".Length).Trim();

				// TODO: We lose the file information because we just read the lines into the list,
				// so the file name won't show up in the diagnostics.
				if(File.Exists(fileName))
				{
					this.lines.Clear();
					this.lines.AddRange(await File.ReadAllLinesAsync(fileName));
				}
				else
				{
					using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
					Console.Out.WriteLine($"File {fileName} does not exist.");
				}
			}
			else if (input == "#showTree")
			{
				this.showTree = !this.showTree;
				Console.Out.WriteLine(this.showTree ? "Showing parse trees." : "Not showing parse trees.");
			}
			else if (input == "#showProgram")
			{
				this.showProgram = !this.showProgram;
				Console.Out.WriteLine(this.showProgram ? "Showing program." : "Not showing program.");
			}
			else if (input == "#cls")
			{
				Console.Clear();
				this.lines.Clear();
			}
			else if (input == "#reset")
			{
				Console.Clear();
				this.lines.Clear();
			}
			else if (input == "#run")
			{
				Repl.Evaluate(this.showProgram, this.showTree, this.lines);
			}
			else
			{
				using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
				Console.Out.WriteLine($"Invalid command: {input}.");
			}
		}

		private static void Evaluate(bool showProgram, bool showTree, List<string> lines)
		{
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

			if (diagnostics.Length > 0)
			{
				DiagnosticsPrinter.Print(diagnostics);
			}
			else
			{
				Repl.RunEvaluator(result);
			}
		}

		private static void RunEvaluator(EvaluationResult evaluation) => 
			new ExecutionEngine(evaluation.Lines, new SecureRandom(), Console.Out, Console.In).Execute();
	}
}