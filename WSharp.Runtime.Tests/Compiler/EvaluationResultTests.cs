using NUnit.Framework;
using System.Collections.Immutable;
using WSharp.Runtime.Compiler;

namespace WSharp.Runtime.Tests.Compiler
{
	public static class EvaluationResultTests
	{
		[Test]
		public static void Create()
		{
			var diagnostics = ImmutableArray<Diagnostic>.Empty;
			var lines = ImmutableArray<Line>.Empty;

			var result = new EvaluationResult(diagnostics, lines);

			Assert.Multiple(() =>
			{
				Assert.That(result.Diagnostics, Is.EqualTo(diagnostics), nameof(result.Diagnostics));
				Assert.That(result.Lines, Is.EqualTo(lines), nameof(result.Lines));
			});
		}
	}
}