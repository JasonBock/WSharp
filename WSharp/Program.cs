using Spackle;
using Spackle.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;
using WSharp.Runtime;

namespace WSharp
{
	public static class Program
	{
		public static async Task Main(FileInfo? file, Interaction interaction = Interaction.Interpret)
		{
			if(interaction == Interaction.Interpret)
			{
				var repl = new Repl();

				if(file != null)
				{
					await repl.RunAsync(file);
				}
				else
				{
					await repl.RunAsync();
				}
			}
			else
			{
				if(file == null)
				{
					using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
					Console.Out.WriteLine($"No file was provided.");
				}
				else if(!file.Exists)
				{
					using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
					Console.Out.WriteLine($"File {file} does not exist.");
				}
				else
				{
					var text = await File.ReadAllTextAsync(file.FullName);
					var tree = await SyntaxTree.LoadAsync(file);
					var compilation = new Compilation(tree);
					var evaluation = compilation.Evaluate();
					var diagnostics = evaluation.Diagnostics;

					if(diagnostics.Length > 0)
					{
						DiagnosticsPrinter.Print(diagnostics);
					}
					else
					{
						// TODO: This is where we'd actually emit something.
						var engine = new ExecutionEngine(evaluation.Lines, new SecureRandom(), Console.Out, Console.In);
						engine.Execute();
					}
				}
			}
		}
	}
}