namespace WSharp.Compiler.Text
{
	public sealed class TextLine
	{
		public TextLine(SourceText text, int start, int length, int lengthIncludingLineBreak) => 
			(this.Text, this.Start, this.Length, this.LengthIncludingLineBreak) =
				(text, start, length, lengthIncludingLineBreak);

		public override string ToString() => this.Text.ToString(this.Span);

		public SourceText Text { get; }
		public int Start { get; }
		public int Length { get; }
		public int End => this.Start + this.Length;
		public int LengthIncludingLineBreak { get; }
		public TextSpan Span => new TextSpan(this.Start, this.Length);
		public TextSpan SpanIncludingLineBreak => new TextSpan(this.Start, this.LengthIncludingLineBreak);
	}
}