using System.Collections.Immutable;
using System.Reflection;

namespace WSharp.Compiler.Symbols;

internal static class BuiltinFunctions
{
	public static readonly FunctionSymbol Again =
		new("again", ImmutableArray.Create(new ParameterSymbol("shouldKeep", TypeSymbol.Boolean)), TypeSymbol.Void);
	public static readonly FunctionSymbol Defer =
		new("defer", ImmutableArray.Create(new ParameterSymbol("shouldDefer", TypeSymbol.Boolean)), TypeSymbol.Void);
	public static readonly FunctionSymbol E =
		new("E", ImmutableArray.Create(new ParameterSymbol("lineNumber", TypeSymbol.Integer)), TypeSymbol.Boolean);
	public static readonly FunctionSymbol N =
		new("N", ImmutableArray.Create(new ParameterSymbol("lineNumber", TypeSymbol.Integer)), TypeSymbol.Integer);
	public static readonly FunctionSymbol Print =
		new("print", ImmutableArray.Create(new ParameterSymbol("value", TypeSymbol.Any)), TypeSymbol.Void);
	public static readonly FunctionSymbol Random =
		new("random", ImmutableArray.Create(new ParameterSymbol("maximum", TypeSymbol.Integer)), TypeSymbol.Integer);
	public static readonly FunctionSymbol Read =
		new("read", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
	public static readonly FunctionSymbol U =
		new("U", ImmutableArray.Create(new ParameterSymbol("unicodeCharacter", TypeSymbol.Integer)), TypeSymbol.String);

	internal static IEnumerable<FunctionSymbol> GetAll() =>
		typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
			.Where(_ => _.FieldType == typeof(FunctionSymbol))
			.Select(_ => (FunctionSymbol)_.GetValue(null)!);
}