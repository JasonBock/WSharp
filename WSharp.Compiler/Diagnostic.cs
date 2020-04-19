using WSharp.Compiler.Text;

namespace WSharp.Compiler
{
	public sealed class Diagnostic
	{
		public Diagnostic(TextSpan span, string message) => 
			(this.Span, this.Message) = (span, message);

		public string Message { get; }
		public TextSpan Span { get; }

		public override string ToString() => this.Message;
	}
}