using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundLineStatement
		: BoundStatement
	{
		public BoundLineStatement(BoundStatement number, List<BoundStatement> statements) =>
			(this.Number, this.Statements) = (number, statements);

		public BoundStatement Number { get; }
		public List<BoundStatement> Statements { get; }

		public override BoundNodeKind Kind => BoundNodeKind.LineStatement;
	}
}