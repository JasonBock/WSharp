using NUnit.Framework;
using System.Collections.Immutable;
using WSharp.Runtime;

namespace WSharp.Compiler.Tests;

internal static class EvaluationResultTests
	{
		[Test]
		public static void Create()
		{
			var diagnostics = ImmutableArray<Diagnostic>.Empty;
			var lines = ImmutableArray<Line>.Empty;

			var result = new EvaluationResult(diagnostics, lines);

		 using (Assert.EnterMultipleScope())
		 {
				Assert.That(result.Diagnostics, Is.EqualTo(diagnostics), nameof(result.Diagnostics));
				Assert.That(result.Lines, Is.EqualTo(lines), nameof(result.Lines));
			}
		}
	}