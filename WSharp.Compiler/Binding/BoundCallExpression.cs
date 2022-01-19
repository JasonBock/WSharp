using System.Collections.Immutable;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding;

internal sealed class BoundCallExpression
	: BoundExpression
{
	public BoundCallExpression(SyntaxNode syntax, FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
		: base(syntax) =>
			(this.Function, this.Arguments) = (function, arguments);

	public override IEnumerable<BoundNode> GetChildren()
	{
		foreach (var argument in this.Arguments)
		{
			yield return argument;
		}
	}

	public override IEnumerable<(string name, object value)> GetProperties() =>
		Enumerable.Empty<(string, object)>();

	public ImmutableArray<BoundExpression> Arguments { get; }
	public FunctionSymbol Function { get; }
	public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
	public override TypeSymbol Type => this.Function.ReturnType;
}