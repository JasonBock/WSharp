using System.Collections.Generic;
using System.Collections.Immutable;

namespace WSharp.Runtime.Compiler
{
	public sealed class EvaluationResult
	{
		public EvaluationResult(Diagnostic[] diagnostics, ImmutableList<Line> lines) => 
			(this.Diagnostics, this.Lines) = (diagnostics, lines);

		public IReadOnlyList<Diagnostic> Diagnostics { get; }
		public ImmutableList<Line> Lines { get; }
	}
}