using System.Collections.Immutable;

namespace WSharp.Compiler
{
	public sealed class EmitResult
	{
		public EmitResult(ImmutableArray<Diagnostic> diagnostics) =>
			this.Diagnostics = diagnostics;

		public ImmutableArray<Diagnostic> Diagnostics { get; }
	}
}