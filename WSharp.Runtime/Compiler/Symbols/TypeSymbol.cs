namespace WSharp.Runtime.Compiler.Symbols
{
	public sealed class TypeSymbol
		: Symbol
	{
		// TODO: Make the symbol names constants and reuse in Lookup().
		public static readonly TypeSymbol Boolean = new TypeSymbol("boolean");
		public static readonly TypeSymbol Error = new TypeSymbol("?");
		public static readonly TypeSymbol Integer = new TypeSymbol("integer");
		public static readonly TypeSymbol String = new TypeSymbol("string");
		public static readonly TypeSymbol Void = new TypeSymbol("void");

		public static TypeSymbol? Lookup(string name) =>
			name switch
			{
				"boolean" => TypeSymbol.Boolean,
				"?" => TypeSymbol.Error,
				"integer" => TypeSymbol.Integer,
				"string" => TypeSymbol.String,
				"void" => TypeSymbol.Void,
				_ => null,
			};

		private TypeSymbol(string name)
			: base(name) { }

		public override SymbolKind Kind => SymbolKind.Type;
	}
}