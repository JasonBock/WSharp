using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundUnaryExpression
	: BoundExpression
{
	public BoundUnaryExpression(SyntaxNode syntax, BoundUnaryOperator @operator, BoundExpression operand)
		: base(syntax) =>
		(this.Operator, this.Operand, this.ConstantValue) =
			(@operator, operand, ConstantFolding.ComputeConstant(@operator, operand));

	public override IEnumerable<BoundNode> GetChildren()
	{
		yield return this.Operand;
	}

	public override IEnumerable<(string name, object value)> GetProperties()
	{
		yield return (nameof(this.Type), this.Type);
	}

	public override BoundConstant? ConstantValue { get; }
	public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
	public BoundExpression Operand { get; }
	public BoundUnaryOperator Operator { get; }
	public override TypeSymbol Type => this.Operator.ResultType;
}