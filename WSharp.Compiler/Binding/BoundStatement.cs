using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	internal abstract class BoundStatement
		: BoundNode
	{
		protected BoundStatement(SyntaxNode syntax)
			: base(syntax) { }
	}
}