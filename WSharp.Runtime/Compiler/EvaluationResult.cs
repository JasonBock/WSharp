using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WSharp.Runtime.Compiler
{
	public sealed class EvaluationResult
	{
		public EvaluationResult(IEnumerable<string> diagnostics, ImmutableList<Line> lines) => 
			(this.Diagnostics, this.Lines) = (diagnostics.ToArray(), lines);

		public IReadOnlyList<string> Diagnostics { get; }
		public ImmutableList<Line> Lines { get; }
	}
}