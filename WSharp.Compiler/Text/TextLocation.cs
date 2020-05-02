using System;

namespace WSharp.Compiler.Text
{
	public readonly struct TextLocation 
		: IEquatable<TextLocation>
	{
		public TextLocation(SourceText text, TextSpan span) => 
			(this.Text, this.Span) = (text, span);

		public static bool operator ==(TextLocation left, TextLocation right) => left.Equals(right);

		public static bool operator !=(TextLocation left, TextLocation right) => !(left == right);

		public override bool Equals(object? obj) => obj is TextLocation other ? this.Equals(other) : false;

		public bool Equals(TextLocation other) => this.GetHashCode() == other.GetHashCode();

		public override int GetHashCode() => HashCode.Combine(this.Text, this.Span);

		public int EndCharacter => this.Span.End - this.Text.Lines[this.StartLine].Start;
		public int EndLine => this.Text.GetLineIndex(this.Span.End);
		public SourceText Text { get; }
		public TextSpan Span { get; }
		public int StartCharacter => this.Span.Start - this.Text.Lines[this.StartLine].Start;
		public int StartLine => this.Text.GetLineIndex(this.Span.Start);
	}
}