using Spackle.Extensions;
using System.Collections.Immutable;
using WSharp.Compiler;
using WSharp.Compiler.Text;

namespace WSharp;

internal static class DiagnosticsPrinter
{
	internal static async Task PrintAsync(ImmutableArray<Diagnostic> diagnostics)
	{
		foreach (var diagnostic in diagnostics
			.Where(_ => _.Location.Text is null))
		{
			using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
			await Console.Error.WriteLineAsync(diagnostic.Message);
			await Console.Out.WriteLineAsync(diagnostic.Message);
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

			await Console.Out.WriteLineAsync();
			using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
			{
				var origin = $"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ";
				await Console.Out.WriteAsync(origin);
				await Console.Out.WriteLineAsync(diagnostic.ToString());
			}

			var lineSpan = line.SpanIncludingLineBreak;
			var prefixSpan = TextSpan.FromBounds(lineSpan.Start, span.Start);
			var suffixSpan = TextSpan.FromBounds(span.End, lineSpan.End);

			var prefix = text.ToString(prefixSpan);
			var error = text.ToString(span);
			var suffix = text.ToString(suffixSpan);

			await Console.Out.WriteAsync(TreePrint.Space);
			await Console.Out.WriteAsync(prefix);

			using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
			{
				await Console.Out.WriteAsync(error);
			}

			await Console.Out.WriteAsync(suffix);
			await Console.Out.WriteLineAsync();
		}

		await Console.Out.WriteLineAsync();
	}
}