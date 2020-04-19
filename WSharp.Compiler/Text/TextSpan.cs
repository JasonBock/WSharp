using System;
using System.Diagnostics.CodeAnalysis;

namespace WSharp.Compiler.Text
{
	public readonly struct TextSpan
		: IComparable<TextSpan>
	{
		public TextSpan(int start, int length) => 
			(this.Start, this.Length) = (start, length);

		public static TextSpan FromBounds(int start, int end)
		{
			var length = end - start;
			return new TextSpan(start, length);
		}

		public override string ToString() => $"{this.Start}..{this.End}";

		public int CompareTo([AllowNull] TextSpan other)
		{
			var start = this.Start.CompareTo(other.Start);

			if(start == 0)
			{
				return this.Length.CompareTo(other.Length);
			}
			else
			{
				return start;
			}
		}

		public int End => this.Start + this.Length;
		public int Length { get; }
		public int Start { get; }
	}
}