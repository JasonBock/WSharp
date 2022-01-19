using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding;

public sealed class Conversion
{
	public static readonly Conversion Explicit = new(ConversionKind.Exists);
	public static readonly Conversion Identity = new(ConversionKind.Exists | ConversionKind.IsIdentity | ConversionKind.IsImplicit);
	public static readonly Conversion Implicit = new(ConversionKind.Exists | ConversionKind.IsImplicit);
	public static readonly Conversion None = new(ConversionKind.None);

	private Conversion(ConversionKind flags) =>
		(this.Exists, this.IsIdentity, this.IsImplicit) =
			(flags.HasFlag(ConversionKind.Exists), flags.HasFlag(ConversionKind.IsIdentity), flags.HasFlag(ConversionKind.IsImplicit));

	public static Conversion Classify(TypeSymbol from, TypeSymbol to)
	{
		if (from == to)
		{
			return Conversion.Identity;
		}

		if (from != TypeSymbol.Void && to == TypeSymbol.Any)
		{
			return Conversion.Implicit;
		}

		if (from == TypeSymbol.Any && to != TypeSymbol.Void)
		{
			return Conversion.Explicit;
		}

		if (from == TypeSymbol.Integer || from == TypeSymbol.Boolean)
		{
			if (to == TypeSymbol.String)
			{
				return Conversion.Explicit;
			}
		}

		if (from == TypeSymbol.String)
		{
			if (to == TypeSymbol.Integer || to == TypeSymbol.Boolean)
			{
				return Conversion.Explicit;
			}
		}

		return Conversion.None;
	}

	public bool Exists { get; }
	public bool IsExplicit => this.Exists && !this.IsImplicit;
	public bool IsIdentity { get; }
	public bool IsImplicit { get; }
}