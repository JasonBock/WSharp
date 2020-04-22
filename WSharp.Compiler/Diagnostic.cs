using WSharp.Compiler.Text;

namespace WSharp.Compiler
{
	public sealed class Diagnostic
	{
		public Diagnostic(TextLocation? location, string message) => 
			(this.Location, this.Message) = (location, message);

		public string Message { get; }
		public TextLocation? Location { get; }

		public override string ToString() => this.Message;
	}
}