using NUnit.Framework;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Tests
{
	public static class EvaluatorTests
	{
		[Test]
		public static void EvaluateText()
		{
			var tree = SyntaxTree.Parse("1 1#3;");
			var binder = new Compilation(tree);
			var result = binder.Evaluate();

			Assert.Multiple(() =>
			{
				Assert.That(result.Diagnostics.Length, Is.EqualTo(0), nameof(result.Diagnostics));
				Assert.That(result.Lines.Length, Is.EqualTo(1), nameof(result.Lines));
			});
		}
	}
}