using Spackle.Extensions;
using System.CommandLine;
using WSharp;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;

var codeFileOption = new Option<FileInfo>("--codeFile")
{
	Description = "The Whenever code file",
	DefaultValueFactory = parseResult => new FileInfo("HelloWorld.ws")
};

var referencesOption = new Option<FileInfo[]?>("--references")
{
	Description = ".NET assembly file reference (if needed)"
};

var outputPathOption = new Option<FileInfo?>("--outputPath")
{
	Description = "The path used to create the EXE (only relevant when interaction mode is \"Compile\")"
};

var interactionOption = new Option<Interaction>("--interaction")
{
	Description = "Specifies if code should be interpreted, or compiled",
	DefaultValueFactory = parseResult => Interaction.Interpret
};

var emitDebuggingOption = new Option<bool>("--emitDebugging")
{
	Description = "An optional flag to specify if a debug symbol should be emitted (only relevant when interaction mode is \"Compile\")"
};

var rootCommand = new RootCommand("Whenever host");
rootCommand.Options.Add(codeFileOption);
rootCommand.Options.Add(referencesOption);
rootCommand.Options.Add(outputPathOption);
rootCommand.Options.Add(interactionOption);
rootCommand.Options.Add(emitDebuggingOption);
rootCommand.SetAction(async parseResult =>
{
	var codeFile = parseResult.GetValue(codeFileOption)!;
	var references = parseResult.GetValue(referencesOption);
	var outputPath = parseResult.GetValue(outputPathOption);
	var interaction = parseResult.GetValue(interactionOption);
	var emitDebugging = parseResult.GetValue(emitDebuggingOption);

	//System.Diagnostics.Debugger.Launch();
	//interaction = Interaction.Compile;
	//file = new FileInfo("HelloWorld.ws");

	if (interaction == Interaction.Interpret)
	{
		var repl = new Repl();

		if (codeFile != null)
		{
			await repl.RunAsync(codeFile);
		}
		else
		{
			await repl.RunAsync();
		}

		return 0;
	}
	else
	{
		if (ValidateParameters(codeFile, references ?? []))
		{
			var targetFile = codeFile;
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
});

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();

static bool ValidateParameters(FileInfo? file, FileInfo[] references)
{
	var errors = new List<string>();

	if (file is null)
	{
		errors.Add($"No file was provided.");
	}
	else if (!file.Exists)
	{
		errors.Add($"File {file} does not exist.");
	}

	foreach (var reference in references)
	{
		if (!reference.Exists)
		{
			errors.Add($"Reference {reference} does not exist.");
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