using System.Collections.Generic;
using System.Linq;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundExpressionStatement
		: BoundStatement
	{
		public BoundExpressionStatement(BoundExpression expression) => 
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