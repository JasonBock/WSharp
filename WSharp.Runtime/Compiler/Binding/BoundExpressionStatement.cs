namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundExpressionStatement
		: BoundStatement
	{
		public BoundExpressionStatement(BoundExpression expression) => 
			this.Expression = expression;

		public BoundExpression Expression { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;
	}
}