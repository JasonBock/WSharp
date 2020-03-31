using NUnit.Framework;
using WSharp.Runtime.Compiler;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler
{
	public static class EvaluatorTests
	{
		[Test]
		public static void EvaluateText()
		{
			var tree = SyntaxTree.Parse("1 2#3;");
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