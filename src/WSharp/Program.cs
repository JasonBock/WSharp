using Spackle.Extensions;
using System.Collections.Immutable;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;

namespace WSharp;

internal static class Program
{
	public static async Task<int> Main(FileInfo? file, FileInfo[]? references, FileInfo? outputPath, Interaction interaction, bool emitDebugging = false)
	{
		//System.Diagnostics.Debugger.Launch();
		//interaction = Interaction.Compile;
		//file = new FileInfo("HelloWorld.ws");

		if (interaction == Interaction.Interpret)
		{
			var repl = new Repl();

			if (file != null)
			{
				await repl.RunAsync(file);
			}
			else
			{
				await repl.RunAsync();
			}

			return 0;
		}
		else
		{
			if (Program.ValidateParameters(file, references ?? []))
			{
				var targetFile = file ?? file!;
				var targetReferences = references ?? [];

				var moduleName = targetFile.Name.Replace(targetFile.Extension, string.Empty);
				outputPath ??= new FileInfo(Path.ChangeExtension(targetFile.Name, ".exe"));

				var tree = await SyntaxTree.LoadAsync(targetFile);

				if (tree.Diagnostics.Length > 0)
				{
					await DiagnosticsPrinter.PrintAsync(tree.Diagnostics);
					return 1;
				}

				var compilation = new Compilation(tree);

				if (compilation.Diagnostics.Count > 0)
				{
					await DiagnosticsPrinter.PrintAsync([.. compilation.Diagnostics]);
					return 1;
				}
				else
				{
					var result = compilation.Emit(moduleName, targetReferences, outputPath, emitDebugging);

					if (result.Diagnostics.Length > 0)
					{
						await DiagnosticsPrinter.PrintAsync(result.Diagnostics);
						return 1;
					}
				}
			}

			return 0;
		}
	}

	private static bool ValidateParameters(FileInfo? file, FileInfo[] references)
	{
		var errors = new List<string>();

		if (file == null)
		{
			errors.Add($"No file was provided.");
		}
		else if (!file.Exists)
		{
			errors.Add($"File {file} does not exist.");
		}

		if (references is { } && references.Length > 0)
		{
			foreach (var reference in references)
			{
				if (!reference.Exists)
				{
					errors.Add($"Reference {reference} does not exist.");
				}
			}
		}

		if (errors.Count > 0)
		{
			using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
			{
				foreach (var error in errors)
				{
					Console.Out.WriteLine(error);
				}
			}

			return false;
		}
		else
		{
			return true;
		}
	}
}