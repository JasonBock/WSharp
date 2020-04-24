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
				.Where(_ => _.Location.Text is null))
			{
				using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
				Console.Out.WriteLine(diagnostic.Message);
			}

			foreach (var diagnostic in diagnostics
				.Where(_ => _.Location.Text is { })
				.OrderBy(_ => _.Location.Text.File?.FullName)
				.ThenBy(_ => _.Location.Span.Start)
				.ThenBy(_ => _.Location.Span.Length))
			{
				var text = diagnostic.Location.Text;
				var fileName = diagnostic.Location.Text.File?.FullName ?? string.Empty;
				var startLine = diagnostic.Location.StartLine + 1;
				var startCharacter = diagnostic.Location.StartCharacter + 1;
				var endLine = diagnostic.Location.EndLine + 1;
				var endCharacter = diagnostic.Location.EndCharacter + 1;

				var span = diagnostic.Location.Span;
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