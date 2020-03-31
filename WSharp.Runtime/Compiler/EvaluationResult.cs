using System.Collections.Immutable;

namespace WSharp.Runtime.Compiler
{
	public sealed class EvaluationResult
	{
		public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<Line> lines) => 
			(this.Diagnostics, this.Lines) = (diagnostics, lines);

		public ImmutableArray<Diagnostic> Diagnostics { get; }
		public ImmutableArray<Line> Lines { get; }
	}
}