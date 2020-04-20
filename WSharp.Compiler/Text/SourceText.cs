using System.Collections.Immutable;
using System.IO;

namespace WSharp.Compiler.Text
{
	public sealed class SourceText
	{
		private SourceText(string text, FileInfo? file) => 
			(this.Text, this.Lines, this.File) = (text, SourceText.ParseLines(this, text), file);

		public char this[int index] => this.Text[index];

		public int Length => this.Text.Length;

		public override string ToString() => this.Text;

		public string ToString(int start, int length) => this.Text.Substring(start, length);

		public string ToString(TextSpan span) => this.Text.Substring(span.Start, span.Length);

		public int GetLineIndex(int position)
		{
			var lower = 0;
			var upper = this.Lines.Length - 1;

			while(lower <= upper)
			{
				var index = lower + (upper - lower) / 2;
				var start = this.Lines[index].Start;

				if(position == start)
				{
					return index;
				}

				if(start > position)
				{
					upper = index - 1;
				}
				else
				{
					lower = index + 1;
				}
			}

			return lower - 1;
		}

		private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
		{
			var lines = ImmutableArray.CreateBuilder<TextLine>();

			var position = 0;
			var lineStart = 0;

			while(position < text.Length)
			{
				var lineBreakWidth = SourceText.GetLineBreakWidth(text, position);

				if(lineBreakWidth == 0)
				{
					position++;
				}
				else
				{
					SourceText.AddLine(lines, sourceText, position, lineStart, lineBreakWidth);

					position += lineBreakWidth;
					lineStart = position;
				}
			}

			if(position >= lineStart)
			{
				SourceText.AddLine(lines, sourceText, position, lineStart, 0);
			}

			return lines.ToImmutable();
		}

		private static void AddLine(ImmutableArray<TextLine>.Builder lines, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
		{
			var lineLength = position - lineStart;
			var lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
			var line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
			lines.Add(line);
		}

		private static int GetLineBreakWidth(string text, int position)
		{
			var character = text[position];
			var lookahead = position + 1 >= text.Length ? '\0' : text[position + 1];

			if(character == '\r' && lookahead == '\n')
			{
				return 2;
			}

			if(character == '\r' || character == '\n')
			{
				return 1;
			}

			return 0;
		}

		public static SourceText From(string text, FileInfo? file = null) => 
			new SourceText(text, file);

		public FileInfo? File { get; }
		private string Text { get; }
		public ImmutableArray<TextLine> Lines { get; }
	}
}