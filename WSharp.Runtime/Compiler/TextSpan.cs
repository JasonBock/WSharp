namespace WSharp.Runtime.Compiler
{
	public readonly struct TextSpan
	{
		public TextSpan(int start, int length) => 
			(this.Start, this.Length) = (start, length);

		public int End => this.Start + this.Length;
		public int Length { get; }
		public int Start { get; }
	}
}