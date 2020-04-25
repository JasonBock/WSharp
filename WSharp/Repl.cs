using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
		private readonly List<MetaCommand> metaCommands = new List<MetaCommand>();
		private readonly List<string> lines = new List<string>();
		private bool showProgram = false;
		private bool showTree = false;

		public Repl()
		{
			foreach (var method in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var attribute = method.GetCustomAttribute<MetaCommandAttribute>();

				if (attribute != null)
				{
					this.metaCommands.Add(new MetaCommand(attribute.Name, attribute.Description, method));
				}
			}
		}

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
			var arguments = new List<string>();
			var inQuotes = false;
			var position = 1;
			var builder = new StringBuilder();

			while (position < input.Length)
			{
				var current = input[position];
				var lookahead = position + 1 >= input.Length ? '\0' : input[position + 1];

				if (char.IsWhiteSpace(current))
				{
					if (!inQuotes)
					{
						CommitPendingArgument();
					}
					else
					{
						builder.Append(current);
					}
				}
				else if (current == '\"')
				{
					if (!inQuotes)
					{
						inQuotes = true;
					}
					else if (lookahead == '\"')
					{
						builder.Append(current);
						position++;
					}
					else
					{
						inQuotes = false;
					}
				}
				else
				{
					builder.Append(current);
				}

				position++;
			}

			CommitPendingArgument();

			void CommitPendingArgument()
			{
				var argument = builder.ToString();

				if (!string.IsNullOrWhiteSpace(argument))
				{
					arguments.Add(argument);
				}

				builder.Clear();
			}

			var commandName = arguments.FirstOrDefault();

			if (arguments.Count > 0)
			{
				arguments.RemoveAt(0);
			}

			var command = this.metaCommands.SingleOrDefault(_ => _.Name == commandName);

			if (command == null)
			{
				using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
					Console.Out.WriteLine($"Invalid command: {input}.");
			}
			else
			{
				var parameters = command.Method.GetParameters();

				if (arguments.Count != parameters.Length)
				{
					var parameterNames = string.Join(" ", parameters.Select(_ => $"<{_.Name}>"));
					using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
						Console.Out.WriteLine($"Invalid number of arguments (given {arguments.Count}, expected {parameters.Length}).");
					Console.Out.WriteLine($"Usage: #{command.Name} {parameterNames}");
				}
				else
				{
					await (Task)command.Method.Invoke(this, arguments.ToArray())!;
				}
			}
		}

		[MetaCommand("loadFile", "Loads the file.")]
		private async Task EvaluateLoadFileAsync(string fileName)
		{
			// TODO: We lose the file information because we just read the lines into the list,
			// so the file name won't show up in the diagnostics.
			if (File.Exists(fileName))
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

		[MetaCommand("run", "Evaluates the program.")]
		private Task EvaluateRun()
		{
			Repl.Evaluate(this.showProgram, this.showTree, this.lines);
			return Task.CompletedTask;
		}

		[MetaCommand("reset", "Clears the screen and the current lines of code.")]
		private async Task EvaluateReset()
		{
			await this.EvaluateClearScreen();
			this.lines.Clear();
		}

		[MetaCommand("cls", "Clears the screen.")]
		private Task EvaluateClearScreen()
		{
			Console.Clear();
			return Task.CompletedTask;
		}

		[MetaCommand("showProgram", "Displays/hides bound tree.")]
		private async Task EvaluateShowProgram()
		{
			this.showProgram = !this.showProgram;
			await Console.Out.WriteLineAsync(this.showProgram ? "Showing program." : "Not showing program.");
		}

		[MetaCommand("showTree", "Displays/hides syntax tree.")]
		private async Task EvaluateShowTree()
		{
			this.showTree = !this.showTree;
			await Console.Out.WriteLineAsync(this.showTree ? "Showing parse trees." : "Not showing parse trees.");
		}

		[MetaCommand("help", "Shows help")]
		private async Task EvaluateHelp()
		{
			var maxNameLength = this.metaCommands.Max(_ => _.Name.Length);

			foreach (var metaCommand in this.metaCommands.OrderBy(_ => _.Name))
			{
				var metaParameters = metaCommand.Method.GetParameters();

				if (metaParameters.Length == 0)
				{
					var paddedName = metaCommand.Name.PadRight(maxNameLength);
					await Console.Out.WriteLineAsync($"{metaCommand.Name.PadRight(maxNameLength)}    {metaCommand.Description}");
				}
				else
				{
					await Console.Out.WriteLineAsync($"{metaCommand.Name} {string.Join(" ", metaParameters.Select(_ => $"<{_.Name}>"))}");
					await Console.Out.WriteLineAsync($"{string.Empty.PadRight(maxNameLength)}    {metaCommand.Description}");
				}
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
			new ExecutionEngine(evaluation.Lines, new SecureRandom(), Console.In, Console.Out).Execute();
	}
}