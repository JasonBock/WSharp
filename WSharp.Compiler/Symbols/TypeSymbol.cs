namespace WSharp.Compiler.Symbols
{
	public sealed class TypeSymbol
		: Symbol
	{
		public const string BooleanName = "boolean";
		public const string ErrorName = "?";
		public const string IntegerName = "integer";
		public const string StringName = "string";
		public const string VoidName = "void";

		public static readonly TypeSymbol Boolean = new TypeSymbol(TypeSymbol.BooleanName);
		public static readonly TypeSymbol Error = new TypeSymbol(TypeSymbol.ErrorName);
		public static readonly TypeSymbol Integer = new TypeSymbol(TypeSymbol.IntegerName);
		public static readonly TypeSymbol String = new TypeSymbol(TypeSymbol.StringName);
		public static readonly TypeSymbol Void = new TypeSymbol(TypeSymbol.VoidName);

		public static TypeSymbol? Lookup(string name) =>
			name switch
			{
				TypeSymbol.BooleanName => TypeSymbol.Boolean,
				TypeSymbol.ErrorName => TypeSymbol.Error,
				TypeSymbol.IntegerName => TypeSymbol.Integer,
				TypeSymbol.StringName => TypeSymbol.String,
				TypeSymbol.VoidName => TypeSymbol.Void,
				_ => null,
			};

		private TypeSymbol(string name)
			: base(name) { }

		public override SymbolKind Kind => SymbolKind.Type;
	}
}