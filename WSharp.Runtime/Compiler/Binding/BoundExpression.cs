using System;

namespace WSharp.Runtime.Compiler.Binding
{
	internal abstract class BoundExpression
		: BoundNode
	{
		public abstract Type Type { get; }
	}
}