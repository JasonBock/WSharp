using System.Collections.Immutable;
using WSharp.Runtime;

namespace WSharp.Compiler;

public sealed class EvaluationResult
{
	public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<Line> lines) =>
		(this.Diagnostics, this.Lines) = (diagnostics, lines);

	public ImmutableArray<Diagnostic> Diagnostics { get; }
	public ImmutableArray<Line> Lines { get; }
}