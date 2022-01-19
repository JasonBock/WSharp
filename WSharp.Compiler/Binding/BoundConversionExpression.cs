using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundConversionExpression
	: BoundExpression
{
	public BoundConversionExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol type)
		: base(syntax) =>
			(this.Expression, this.Type) = (expression, type);

	public override IEnumerable<BoundNode> GetChildren()
	{
		yield return this.Expression;
	}

	public override IEnumerable<(string name, object value)> GetProperties() =>
		Enumerable.Empty<(string, object)>();

	public BoundExpression Expression { get; }
	public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression;
	public override TypeSymbol Type { get; }
}