namespace WSharp.Compiler.Symbols;

public sealed class ParameterSymbol
	: Symbol
{
	public ParameterSymbol(string name, TypeSymbol type)
		: base(name) => this.Type = type;

	public override SymbolKind Kind => SymbolKind.Parameter;

	public TypeSymbol Type { get; }
}