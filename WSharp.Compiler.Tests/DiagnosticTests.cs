using NUnit.Framework;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Tests.Compiler
{
	public static class DiagnosticTests
	{
		[Test]
		public static void Create()
		{
			var span = new TextSpan(3, 4);
			var message = "diagnostic";
			var diagnostic = new Diagnostic(span, message);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.Span, Is.EqualTo(span), nameof(diagnostic.Span));
				Assert.That(diagnostic.Message, Is.EqualTo(message), nameof(diagnostic.Message));
			});
		}
	}
}