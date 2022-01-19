using System.Reflection;
using System.Text;
using Spackle;
using Spackle.Extensions;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;
using WSharp.Runtime;

namespace WSharp;

internal sealed class Repl
{
	private readonly List<MetaCommand> metaCommands = new();
	private readonly List<string> lines = new();
	private bool showProgram;
	private bool showTree;

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
		await this.EvaluateMetaCommandAsync($"#runFile {file.FullName}").ConfigureAwait(false);
		await this.RunAsync().ConfigureAwait(false);
	}

	public async Task RunAsync()
	{
		while (true)
		{
			using (ConsoleColor.Green.Bind(() => Console.ForegroundColor))
			{
				await Console.Out.WriteAsync("» ").ConfigureAwait(false);
			}

			var input = await Console.In.ReadLineAsync().ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(input))
			{
				if (input.StartsWith("#", StringComparison.InvariantCulture))
				{
					await this.EvaluateMetaCommandAsync(input).ConfigureAwait(false);
					continue;
				}
				else
				{
					this.lines.Add(input);
				}
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
				await Console.Out.WriteLineAsync($"Invalid command: {input}.").ConfigureAwait(false);
		}
		else
		{
			var parameters = command.Method.GetParameters();

			if (arguments.Count != parameters.Length)
			{
				var parameterNames = string.Join(" ", parameters.Select(_ => $"<{_.Name}>"));
				using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
					await Console.Out.WriteLineAsync($"Invalid number of arguments (given {arguments.Count}, expected {parameters.Length}).").ConfigureAwait(false);
				await Console.Out.WriteLineAsync($"Usage: #{command.Name} {parameterNames}").ConfigureAwait(false);
			}
			else
			{
				await ((Task)command.Method.Invoke(this, arguments.ToArray())!).ConfigureAwait(false);
			}
		}
	}

	[MetaCommand("runFile", "Runs the file.")]
	private async Task EvaluateLoadFileAsync(string fileName)
	{
		if (File.Exists(fileName))
		{
			this.lines.Clear();
			await this.EvaluateAsync(await SyntaxTree.LoadAsync(new FileInfo(fileName)).ConfigureAwait(false)).ConfigureAwait(false);
		}
		else
		{
			using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
				await Console.Out.WriteLineAsync($"File {fileName} does not exist.").ConfigureAwait(false);
		}
	}

	[MetaCommand("run", "Evaluates the program.")]
	private async Task EvaluateRunAsync() => 
		await this.EvaluateAsync(this.lines).ConfigureAwait(false);

	[MetaCommand("reset", "Clears the screen and the current lines of code.")]
	private Task EvaluateReset()
	{
		Console.Clear();
		this.lines.Clear();
		return Task.CompletedTask;
	}

	[MetaCommand("exit", "Exits the program.")]
#pragma warning disable CA1822
	private Task EvaluateExitAsync()
#pragma warning restore CA1822
	{
		Environment.Exit(0);
		return Task.CompletedTask;
	}

	[MetaCommand("showProgram", "Displays/hides bound tree.")]
	private async Task EvaluateShowProgram()
	{
		this.showProgram = !this.showProgram;
		await Console.Out.WriteLineAsync(this.showProgram ? "Showing program." : "Not showing program.").ConfigureAwait(false);
	}

	[MetaCommand("showTree", "Displays/hides syntax tree.")]
	private async Task EvaluateShowTree()
	{
		this.showTree = !this.showTree;
		await Console.Out.WriteLineAsync(this.showTree ? "Showing parse trees." : "Not showing parse trees.").ConfigureAwait(false);
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
				await Console.Out.WriteLineAsync($"{paddedName}    {metaCommand.Description}").ConfigureAwait(false);
			}
			else
			{
				await Console.Out.WriteLineAsync($"{metaCommand.Name} {string.Join(" ", metaParameters.Select(_ => $"<{_.Name}>"))}").ConfigureAwait(false);
				await Console.Out.WriteLineAsync($"{string.Empty.PadRight(maxNameLength)}    {metaCommand.Description}").ConfigureAwait(false);
			}
		}
	}

	private async Task EvaluateAsync(SyntaxTree tree)
	{
		if (tree.Diagnostics.Length > 0)
		{
			await DiagnosticsPrinter.PrintAsync(tree.Diagnostics).ConfigureAwait(false);
		}
		else
		{
			var compilation = new Compilation(tree);
			var result = compilation.Evaluate();
			var diagnostics = result.Diagnostics;

			if (this.showTree)
			{
				await Console.Out.WriteLineAsync("Tree:").ConfigureAwait(false);
				using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
				{
					tree.Root.WriteTo(Console.Out);
				}
			}

			if (this.showProgram)
			{
				await Console.Out.WriteLineAsync("Program:").ConfigureAwait(false);
				using (ConsoleColor.DarkGray.Bind(() => Console.ForegroundColor))
				{
					compilation.EmitTree(Console.Out);
				}
			}

			if (diagnostics.Length > 0)
			{
				await DiagnosticsPrinter.PrintAsync(diagnostics).ConfigureAwait(false);
			}
			else
			{
				Repl.RunEvaluator(result);
			}
		}
	}

	private async Task EvaluateAsync(List<string> lines) =>
		await this.EvaluateAsync(SyntaxTree.Parse(string.Join(Environment.NewLine, lines))).ConfigureAwait(false);

	private static void RunEvaluator(EvaluationResult evaluation)
	{
		using var random = new SecureRandom();
		new ExecutionEngine(evaluation.Lines, random, Console.In, Console.Out).Execute();
	}
}