using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundUpdateLineCountExpression
	: BoundExpression
{
	public BoundUpdateLineCountExpression(SyntaxNode syntax, BoundExpression left, BoundUpdateLineCountOperatorKind operatorKind, BoundExpression right)
		: base(syntax) =>
			(this.Left, this.OperatorKind, this.Right) = (left, operatorKind, right);

	public override IEnumerable<BoundNode> GetChildren()
	{
		yield return this.Left;
		yield return this.Right;
	}

	public override IEnumerable<(string name, object value)> GetProperties()
	{
		yield return (nameof(this.Type), this.Type);
	}

	public override BoundNodeKind Kind => BoundNodeKind.UpdateLineCountExpression;
	public BoundExpression Left { get; }
	public BoundUpdateLineCountOperatorKind OperatorKind { get; }
	public BoundExpression Right { get; }
	public override TypeSymbol Type => this.Left.Type;
}