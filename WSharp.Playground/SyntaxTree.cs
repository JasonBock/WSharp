using System.Collections.Generic;
using System.Linq;

namespace WSharp.Playground
{
	public sealed class SyntaxTree
	{
		public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFIleToken) => 
			(this.Diagnostics, this.Root, this.EndOfFIleToken) = (diagnostics.ToArray(), root, endOfFIleToken);

		public ExpressionSyntax Root { get; }
		public SyntaxToken EndOfFIleToken { get; }
		public IReadOnlyList<string> Diagnostics { get; }
	}
}