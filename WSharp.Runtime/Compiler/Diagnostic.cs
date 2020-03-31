using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler
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