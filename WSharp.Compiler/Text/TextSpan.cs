using Spackle.Extensions;

namespace WSharp.Compiler.Text;

public readonly struct TextSpan
	 : IEquatable<TextSpan>
{
	public TextSpan(int start, int length) =>
		(this.Start, this.Length) = (start, length);

	public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

	public static bool operator !=(TextSpan left, TextSpan right) => !(left == right);

	public override bool Equals(object? obj) => obj is TextSpan other && this.Equals(other);

	public bool Equals(TextSpan other) => this.GetHashCode() == other.GetHashCode();

	public static TextSpan FromBounds(int start, int end) => new(start, end - start);

	public override int GetHashCode() => HashCode.Combine(this.Start, this.Length);

	public bool OverlapsWith(TextSpan span) => (this.Start..this.End).Intersect(span.Start..span.End) is { };

	public override string ToString() => $"{this.Start}..{this.End}";

	public int End => this.Start + this.Length;
	public int Length { get; }
	public int Start { get; }
}