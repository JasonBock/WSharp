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
			await Console.Error.WriteLineAsync(diagnostic.Message).ConfigureAwait(false);
			await Console.Out.WriteLineAsync(diagnostic.Message).ConfigureAwait(false);
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

			await Console.Out.WriteLineAsync().ConfigureAwait(false);
			using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
			{
				var origin = $"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ";
				await Console.Out.WriteAsync(origin).ConfigureAwait(false);
				await Console.Out.WriteLineAsync(diagnostic.ToString()).ConfigureAwait(false);
			}

			var lineSpan = line.SpanIncludingLineBreak;
			var prefixSpan = TextSpan.FromBounds(lineSpan.Start, span.Start);
			var suffixSpan = TextSpan.FromBounds(span.End, lineSpan.End);

			var prefix = text.ToString(prefixSpan);
			var error = text.ToString(span);
			var suffix = text.ToString(suffixSpan);

			await Console.Out.WriteAsync(TreePrint.Space).ConfigureAwait(false);
			await Console.Out.WriteAsync(prefix).ConfigureAwait(false);

			using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
			{
				await Console.Out.WriteAsync(error).ConfigureAwait(false);
			}

			await Console.Out.WriteAsync(suffix).ConfigureAwait(false);
			await Console.Out.WriteLineAsync().ConfigureAwait(false);
		}

		await Console.Out.WriteLineAsync().ConfigureAwait(false);
	}
}