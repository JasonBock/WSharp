using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class Conversion
	{
		// TODO: Consider using a bit flag, ConversionSettings, to make the construction and state
		// easier to read.
		public static readonly Conversion Explicit = new Conversion(exists: true, isIdentity: false, isImplicit: false);
		public static readonly Conversion Identity = new Conversion(exists: true, isIdentity: true, isImplicit: true);
		public static readonly Conversion Implicit = new Conversion(exists: true, isIdentity: false, isImplicit: true);
		public static readonly Conversion None = new Conversion(exists: false, isIdentity: false, isImplicit: false);

		private Conversion(bool exists, bool isIdentity, bool isImplicit) =>
			(this.Exists, this.IsIdentity, this.IsImplicit) =
				(exists, isIdentity, isImplicit);

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