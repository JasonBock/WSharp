namespace WSharp.Runtime.Compiler.Symbols
{
	public sealed class ParameterSymbol
		: Symbol
	{
		public ParameterSymbol(string name, TypeSymbol type)
			: base(name) { }

		public override SymbolKind Kind => SymbolKind.Parameter;
	}
}