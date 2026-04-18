using System.Reflection;

namespace WSharp.Compiler.Symbols;

internal static class BuiltinFunctions
{
	public static readonly FunctionSymbol Again =
		new("again", [new ParameterSymbol("shouldKeep", TypeSymbol.Boolean)], TypeSymbol.Void);
	public static readonly FunctionSymbol Defer =
		new("defer", [new ParameterSymbol("shouldDefer", TypeSymbol.Boolean)], TypeSymbol.Void);
	public static readonly FunctionSymbol E =
		new("E", [new ParameterSymbol("lineNumber", TypeSymbol.Integer)], TypeSymbol.Boolean);
	public static readonly FunctionSymbol N =
		new("N", [new ParameterSymbol("lineNumber", TypeSymbol.Integer)], TypeSymbol.Integer);
	public static readonly FunctionSymbol Print =
		new("print", [new ParameterSymbol("value", TypeSymbol.Any)], TypeSymbol.Void);
	public static readonly FunctionSymbol Random =
		new("random", [new ParameterSymbol("maximum", TypeSymbol.Integer)], TypeSymbol.Integer);
	public static readonly FunctionSymbol Read =
		new("read", [], TypeSymbol.String);
	public static readonly FunctionSymbol U =
		new("U", [new ParameterSymbol("unicodeCharacter", TypeSymbol.Integer)], TypeSymbol.String);

	internal static IEnumerable<FunctionSymbol> GetAll() =>
		typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
			.Where(_ => _.FieldType == typeof(FunctionSymbol))
			.Select(_ => (FunctionSymbol)_.GetValue(null)!);
}