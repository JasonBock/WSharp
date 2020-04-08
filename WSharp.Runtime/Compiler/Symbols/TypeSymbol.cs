namespace WSharp.Runtime.Compiler.Symbols
{
	public sealed class TypeSymbol
		: Symbol
	{
		public static readonly TypeSymbol Boolean = new TypeSymbol("boolean");
		public static readonly TypeSymbol Error = new TypeSymbol("?");
		public static readonly TypeSymbol Number = new TypeSymbol("number");
		public static readonly TypeSymbol String = new TypeSymbol("string");
		public static readonly TypeSymbol Void = new TypeSymbol("void");

		private TypeSymbol(string name)
			: base(name) { }

		public override SymbolKind Kind => SymbolKind.Type;
	}
}