using System.Collections.Generic;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;

namespace WSharp.Compiler.Binding
{
	internal sealed class BoundUnaryUpdateLineCountExpression
		: BoundExpression
	{
		public BoundUnaryUpdateLineCountExpression(SyntaxNode syntax, BoundExpression lineNumber) 
			: base(syntax) =>
				this.LineNumber = lineNumber;

		public override IEnumerable<BoundNode> GetChildren()
		{
			yield return this.LineNumber;
		}

		public override IEnumerable<(string name, object value)> GetProperties()
		{
			yield return (nameof(this.Type), this.Type);
		}

		public override BoundNodeKind Kind => BoundNodeKind.UnaryUpdateLineCountExpression;
		public BoundExpression LineNumber { get; }
		public override TypeSymbol Type => this.LineNumber.Type;
	}
}