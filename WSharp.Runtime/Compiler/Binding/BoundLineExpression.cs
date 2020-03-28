using System;
using System.Collections.Generic;
using System.Numerics;

namespace WSharp.Runtime.Compiler.Binding
{
	internal sealed class BoundLineExpression
		: BoundExpression
	{
		public BoundLineExpression(BoundExpression number, List<BoundExpression> expressions) =>
			(this.Number, this.Expressions) = (number, expressions);

		public BoundExpression Number { get; }
		public List<BoundExpression> Expressions { get; }

		public override Type Type => typeof(BigInteger);

		public override BoundNodeKind Kind => BoundNodeKind.LineExpression;
	}
}