using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class Conversion
	{
		public static readonly Conversion Explicit = new Conversion(ConversionFlags.Exists);
		public static readonly Conversion Identity = new Conversion(ConversionFlags.Exists | ConversionFlags.IsIdentity | ConversionFlags.IsImplicit);
		public static readonly Conversion Implicit = new Conversion(ConversionFlags.Exists | ConversionFlags.IsImplicit);
		public static readonly Conversion None = new Conversion(ConversionFlags.None);

		private Conversion(ConversionFlags flags) =>
			(this.Exists, this.IsIdentity, this.IsImplicit) =
				(flags.HasFlag(ConversionFlags.Exists), flags.HasFlag(ConversionFlags.IsIdentity), flags.HasFlag(ConversionFlags.IsImplicit));

		public static Conversion Classify(TypeSymbol from, TypeSymbol to)
		{
			if(from == to)
			{
				return Conversion.Identity;
			}

			if(from == TypeSymbol.Integer || from == TypeSymbol.Boolean)
			{
				if(to == TypeSymbol.String)
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
}