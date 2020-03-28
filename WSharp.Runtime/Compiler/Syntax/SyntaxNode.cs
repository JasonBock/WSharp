using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public abstract class SyntaxNode
	{
		public abstract IEnumerable<SyntaxNode> GetChildren();

		public abstract SyntaxKind Kind { get; }
	}
}