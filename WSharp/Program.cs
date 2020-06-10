using Spackle.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;

namespace WSharp
{
	public static class Program
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
					await repl.RunAsync(file).ConfigureAwait(false);
				}
				else
				{
					await repl.RunAsync().ConfigureAwait(false);
				}

				return 0;
			}
			else
			{
				if (Program.ValidateParameters(file, references ?? Array.Empty<FileInfo>()))
				{
					var targetFile = file ?? file!;
					var targetReferences = references ?? Array.Empty<FileInfo>();

					var moduleName = targetFile.Name.Replace(targetFile.Extension, string.Empty);
					outputPath ??= new FileInfo(Path.ChangeExtension(targetFile.Name, ".exe"));

					var tree = await SyntaxTree.LoadAsync(targetFile).ConfigureAwait(false);

					if (tree.Diagnostics.Length > 0)
					{
						DiagnosticsPrinter.Print(tree.Diagnostics);
						return 1;
					}

					var compilation = new Compilation(tree);

					if (compilation.Diagnostics.Count > 0)
					{
						DiagnosticsPrinter.Print(compilation.Diagnostics.ToImmutableArray());
						return 1;
					}
					else
					{
						var result = compilation.Emit(moduleName, targetReferences, outputPath, emitDebugging);

						if (result.Diagnostics.Length > 0)
						{
							DiagnosticsPrinter.Print(result.Diagnostics);
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

			if(references is { } && references.Length > 0)
			{
				foreach(var reference in references)
				{
					if(!reference.Exists)
					{
						errors.Add($"Reference {reference} does not exist.");
					}
				}
			}

			if (errors.Count > 0)
			{
				using (ConsoleColor.Red.Bind(() => Console.ForegroundColor))
				foreach (var error in errors)
				{
					Console.Out.WriteLine(error);
				}

				return false;
			}
			else
			{
				return true;
			}
		}
	}
}