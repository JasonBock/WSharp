using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	internal abstract class BoundExpression
		: BoundNode
	{
		protected BoundExpression(SyntaxNode syntax)
			: base(syntax) { }

		public virtual BoundConstant? ConstantValue { get; }
		public abstract TypeSymbol Type { get; }
	}
}