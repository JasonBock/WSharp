using System.Collections.Generic;
using WSharp.Runtime.Compiler.Symbols;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundUnaryExpression
		: BoundExpression
	{
		public BoundUnaryExpression(BoundUnaryOperator @operator, BoundExpression operand) => 
			(this.Operator, this.Operand) = (@operator, operand);

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.Operand;
		}

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
		}

		public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
		public BoundExpression Operand { get; }
		public BoundUnaryOperator Operator { get; }
		public override TypeSymbol Type => this.Operator.ResultType;
	}
}