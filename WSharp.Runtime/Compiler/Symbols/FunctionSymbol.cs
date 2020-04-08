using System.Collections.Immutable;

namespace WSharp.Runtime.Compiler.Symbols
{
	internal static class BuiltinFunctions
	{
		public static readonly FunctionSymbol Read = new FunctionSymbol(
			"read", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
		public static readonly FunctionSymbol Print = new FunctionSymbol(
			"print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
	}

	public sealed class FunctionSymbol
		: Symbol
	{
		public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType)
			: base(name) => this.ReturnType = returnType;

		public override SymbolKind Kind => SymbolKind.Function;

		public TypeSymbol ReturnType { get; }
	}
}