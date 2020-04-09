using System.Collections.Immutable;

namespace WSharp.Runtime.Compiler.Symbols
{
	public sealed class FunctionSymbol
		: Symbol
	{
		public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType)
			: base(name) =>
				(this.Parameters, this.ReturnType) = (parameters, returnType);

		public override SymbolKind Kind => SymbolKind.Function;

		public ImmutableArray<ParameterSymbol> Parameters { get; }
		public TypeSymbol ReturnType { get; }
	}
}