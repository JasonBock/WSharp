using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests;

internal static class EvaluatorTests
{
	[Test]
	public static void EvaluateText()
	{
		var tree = SyntaxTree.Parse("1 1#3;");
		var binder = new Compilation(tree);
		var result = binder.Evaluate();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result.Diagnostics, Is.Empty, nameof(result.Diagnostics));
			Assert.That(result.Lines, Has.Length.EqualTo(1), nameof(result.Lines));
		}
	}
}