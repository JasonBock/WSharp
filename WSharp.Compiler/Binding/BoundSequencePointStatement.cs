using System.Collections.Generic;
using WSharp.Compiler.Syntax;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Binding
{
	internal sealed class BoundSequencePointStatement
		: BoundStatement
	{
		public BoundSequencePointStatement(SyntaxNode syntax, BoundStatement statement, TextLocation location)
			: base(syntax) => (this.Statement, this.Location) = (statement, location);

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.Statement;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SequencePointStatement;

		public TextLocation Location { get; }
		public BoundStatement Statement { get; }
	}
}