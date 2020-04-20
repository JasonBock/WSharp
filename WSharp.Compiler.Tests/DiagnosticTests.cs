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
			var location = new TextLocation(SourceText.From("1"), span);
			var diagnostic = new Diagnostic(location, message);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.Location, Is.EqualTo(location), nameof(diagnostic.Location));
				Assert.That(diagnostic.Message, Is.EqualTo(message), nameof(diagnostic.Message));
			});
		}
	}
}