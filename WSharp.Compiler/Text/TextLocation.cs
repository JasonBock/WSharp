namespace WSharp.Compiler.Text
{
	public struct TextLocation
	{
		public TextLocation(SourceText text, TextSpan span) => 
			(this.Text, this.Span) = (text, span);

		public int EndCharacter => this.Span.End - this.Text.Lines[this.StartLine].Start;
		public int EndLine => this.Text.GetLineIndex(this.Span.End);
		public SourceText Text { get; }
		public TextSpan Span { get; }
		public int StartCharacter => this.Span.Start - this.Text.Lines[this.StartLine].Start;
		public int StartLine => this.Text.GetLineIndex(this.Span.Start);
	}
}