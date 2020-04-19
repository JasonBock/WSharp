using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace WSharp.Compiler.Symbols
{
	internal static class BuiltinFunctions
	{
		public static readonly FunctionSymbol Again = new FunctionSymbol(
			"again", ImmutableArray.Create(new ParameterSymbol("shouldKeep", TypeSymbol.Boolean)), TypeSymbol.Void);
		public static readonly FunctionSymbol Defer = new FunctionSymbol(
			"defer", ImmutableArray.Create(new ParameterSymbol("shouldDefer", TypeSymbol.Boolean)), TypeSymbol.Void);
		public static readonly FunctionSymbol Exists = new FunctionSymbol(
			"exists", ImmutableArray.Create(new ParameterSymbol("lineNumber", TypeSymbol.Integer)), TypeSymbol.Boolean);
		public static readonly FunctionSymbol N = new FunctionSymbol(
			"N", ImmutableArray.Create(new ParameterSymbol("lineNumber", TypeSymbol.Integer)), TypeSymbol.Integer);
		public static readonly FunctionSymbol Print = new FunctionSymbol(
			"print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.String)), TypeSymbol.Void);
		public static readonly FunctionSymbol Random = new FunctionSymbol(
			"random", ImmutableArray.Create(new ParameterSymbol("maximum", TypeSymbol.Integer)), TypeSymbol.Integer);
		public static readonly FunctionSymbol Read = new FunctionSymbol(
			"read", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
		public static readonly FunctionSymbol U = new FunctionSymbol(
			"U", ImmutableArray.Create(new ParameterSymbol("unicodeCharacter", TypeSymbol.Integer)), TypeSymbol.String);

		internal static IEnumerable<FunctionSymbol> GetAll() =>
			typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(_ => _.FieldType == typeof(FunctionSymbol))
				.Select(_ => (FunctionSymbol)_.GetValue(null)!);
	}
}