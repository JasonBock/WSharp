using System.Collections.Immutable;

namespace WSharp.Compiler.Emit;

public sealed class EmitResult
{
	public EmitResult(ImmutableArray<Diagnostic> diagnostics) =>
		this.Diagnostics = diagnostics;

	public ImmutableArray<Diagnostic> Diagnostics { get; }
}