using NUnit.Framework;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Tests.Symbols;

internal static class ParameterSymbolTests
{
	[Test]
	public static void Create()
	{
		var name = "a";
		var type = TypeSymbol.Boolean;

		var parameter = new ParameterSymbol(name, type);

	  using (Assert.EnterMultipleScope())
	  {
			Assert.That(parameter.Name, Is.EqualTo(name), nameof(parameter.Name));
			Assert.That(parameter.Type, Is.EqualTo(type), nameof(parameter.Type));
		}
	}
}