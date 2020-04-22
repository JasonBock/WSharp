using Spackle.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Compiler;
using WSharp.Compiler.Text;

namespace WSharp
{
	internal static class DiagnosticsPrinter
	{
		internal static void Print(ImmutableArray<Diagnostic> diagnostics)
		{
			foreach (var diagnostic in diagnostics
				.Where(_ => _.Location is null))
			{
				using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
				Console.Out.WriteLine(diagnostic.Message);
			}

			foreach (var diagnostic in diagnostics
				.Where(_ => _.Location is { })
				.OrderBy(_ => _.Location!.Value.Text.File?.FullName)
				.ThenBy(_ => _.Location!.Value.Span.Start)
				.ThenBy(_ => _.Location!.Value.Span.Length))
			{
				var text = diagnostic.Location!.Value.Text;
				var fileName = diagnostic.Location!.Value.Text.File?.FullName ?? string.Empty;
				var startLine = diagnostic.Location!.Value.StartLine + 1;
				var startCharacter = diagnostic.Location!.Value.StartCharacter + 1;
				var endLine = diagnostic.Location!.Value.EndLine + 1;
				var endCharacter = diagnostic.Location!.Value.EndCharacter + 1;

				var span = diagnostic.Location!.Value.Span;
				var lineIndex = text.GetLineIndex(span.Start);
				var line = text.Lines[lineIndex];

				Console.Out.WriteLine();
				using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
				{
					Console.Out.Write($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ");
					Console.Out.WriteLine(diagnostic);
				}

				var prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
				var suffixSpan = TextSpan.FromBounds(span.End, line.End);

				var prefix = text.ToString(prefixSpan);
				var error = text.ToString(span);
				var suffix = text.ToString(suffixSpan);

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
	}
}