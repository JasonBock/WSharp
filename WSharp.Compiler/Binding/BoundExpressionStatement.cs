using System.Collections.Generic;
using System.Linq;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundExpressionStatement
		: BoundStatement
	{
		internal BoundExpressionStatement(BoundExpression expression) => 
			this.Expression = expression;

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.Expression;
		}

		public override IEnumerable<(string name, object value)> GetProperties() =>
			Enumerable.Empty<(string, object)>();

		public BoundExpression Expression { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
	}
}