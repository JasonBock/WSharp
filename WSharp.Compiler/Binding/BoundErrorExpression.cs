using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundErrorExpression
	: BoundExpression
{
	// TODO: Should this accept an array of bound nodes?
	public BoundErrorExpression(SyntaxNode syntax)
		: base(syntax) { }

	public override IEnumerable<BoundNode> GetChildren() =>
		Enumerable.Empty<BoundNode>();

	public override IEnumerable<(string name, object value)> GetProperties() =>
		Enumerable.Empty<(string, object)>();

	public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
	public override TypeSymbol Type => TypeSymbol.Error;
}