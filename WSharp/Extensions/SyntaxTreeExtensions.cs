using Spackle.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Compiler;
using WSharp.Compiler.Syntax;

namespace WSharp.Extensions
{
	internal static class SyntaxTreeExtensions
	{
		internal static void PrintDiagnostics(this SyntaxTree tree, ImmutableArray<Diagnostic> diagnostics)
		{
			var text = tree.Text;

			foreach (var diagnostic in diagnostics.OrderBy(_ => _.Span))
			{
				var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
				var lineNumber = lineIndex + 1;
				var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;

				Console.Out.WriteLine();
				using (ConsoleColor.DarkRed.Bind(() => Console.ForegroundColor))
				{
					Console.Out.WriteLine($"({lineNumber}, {character}): {diagnostic}");
				}

				var textLine = text.Lines[text.GetLineIndex(diagnostic.Span.Start)];
				var prefix = text.ToString(textLine.SpanIncludingLineBreak.Start, diagnostic.Span.Start - textLine.SpanIncludingLineBreak.Start);
				var error = text.ToString(diagnostic.Span.Start, diagnostic.Span.Length);
				var suffix = text.ToString(diagnostic.Span.End, textLine.SpanIncludingLineBreak.End - diagnostic.Span.End);

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