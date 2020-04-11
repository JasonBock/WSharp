using NUnit.Framework;
using System.Collections.Immutable;
using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Tests.Compiler.Symbols
{
	public static class FunctionSymbolTests
	{
		[Test]
		public static void Create()
		{
			var name = "a";
			var parameter = new ParameterSymbol("b", TypeSymbol.Boolean);
			var returnType = TypeSymbol.Void;

			var function = new FunctionSymbol(name, ImmutableArray.Create<ParameterSymbol>(parameter), returnType);

			Assert.Multiple(() =>
			{
				Assert.That(function.Name, Is.EqualTo(name), nameof(function.Name));
				Assert.That(function.ReturnType, Is.EqualTo(returnType), nameof(function.ReturnType));
				Assert.That(function.Parameters.Length, Is.EqualTo(1), nameof(function.Parameters.Length));
				Assert.That(function.Parameters[0], Is.EqualTo(parameter), nameof(function.Parameters));
			});
		}
	}
}