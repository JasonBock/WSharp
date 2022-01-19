using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundLineStatement
	: BoundStatement
{
	public BoundLineStatement(SyntaxNode syntax, BoundStatement number, List<BoundStatement> statements)
		: base(syntax) =>
			(this.Number, this.Statements) = (number, statements);

	public override IEnumerable<BoundNode> GetChildren()
	{
		yield return this.Number;

		foreach (var statement in this.Statements)
		{
			yield return statement;
		}
	}

	public override IEnumerable<(string name, object value)> GetProperties() =>
		Enumerable.Empty<(string, object)>();

	public override BoundNodeKind Kind => BoundNodeKind.LineStatement;
	public BoundStatement Number { get; }
	public List<BoundStatement> Statements { get; }
}