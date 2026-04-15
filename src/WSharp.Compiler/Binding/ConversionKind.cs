namespace WSharp.Compiler.Binding;

[Flags]
public enum ConversionKind
{
	None = 0,
	Exists = 1,
	IsIdentity = 2,
	IsImplicit = 4
}