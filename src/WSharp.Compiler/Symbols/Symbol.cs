namespace WSharp.Compiler.Symbols;

public abstract class Symbol
{
	private protected Symbol(string name) =>
		this.Name = name ?? throw new ArgumentNullException(nameof(name));

	public override string ToString() => this.Name;

	public abstract SymbolKind Kind { get; }
	public string Name { get; }
}