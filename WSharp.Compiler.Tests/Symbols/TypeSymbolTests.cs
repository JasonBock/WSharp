using NUnit.Framework;
using System;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Tests.Compiler.Symbols
{
	public static class TypeSymbolTests
	{
		[Test]
		public static void LookupUnknownTypeSymbol() =>
			Assert.That(TypeSymbol.Lookup(Guid.NewGuid().ToString()), Is.Null);

		[TestCase(TypeSymbol.BooleanName)]
		[TestCase(TypeSymbol.ErrorName)]
		[TestCase(TypeSymbol.IntegerName)]
		[TestCase(TypeSymbol.StringName)]
		[TestCase(TypeSymbol.VoidName)]
		public static void VerifyTypeSymbol(string typeName)
		{
			var symbol = TypeSymbol.Lookup(typeName)!;

			Assert.Multiple(() =>
			{
				Assert.That(symbol.Name, Is.EqualTo(typeName), nameof(symbol.Name));
				Assert.That(symbol.Kind, Is.EqualTo(SymbolKind.Type), nameof(symbol.Kind));
			});
		}
	}
}