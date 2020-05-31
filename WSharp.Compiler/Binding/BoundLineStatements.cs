using System.Collections.Generic;
using System.Linq;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	internal sealed class BoundLineStatements
		: BoundStatement
	{
		public BoundLineStatements(SyntaxNode syntax, List<BoundLineStatement> lineStatements)
			: base(syntax) =>
				this.LineStatements = lineStatements;

		public override IEnumerable<BoundNode> GetChildren()
		{
			foreach(var statement in this.LineStatements)
			{
				yield return statement;
			}
		}

		public override IEnumerable<(string name, object value)> GetProperties() =>
			Enumerable.Empty<(string, object)>();

		public override BoundNodeKind Kind => BoundNodeKind.LineStatements;
		public List<BoundLineStatement> LineStatements { get; }
	}
}