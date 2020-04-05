﻿namespace WSharp.Runtime.Compiler.Text
{
	public readonly struct TextSpan
	{
		public TextSpan(int start, int length) => 
			(this.Start, this.Length) = (start, length);

		public static TextSpan FromBounds(int start, int end)
		{
			var length = end - start;
			return new TextSpan(start, length);
		}

		public override string ToString() => $"{this.Start}..{this.End}";
		
		public int End => this.Start + this.Length;
		public int Length { get; }
		public int Start { get; }
	}
}