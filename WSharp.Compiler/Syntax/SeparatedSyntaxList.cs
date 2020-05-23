using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WSharp.Compiler.Syntax
{
	public sealed class SeparatedSyntaxList<T>
		: IEnumerable<T>
		where T : SyntaxNode
	{
		internal SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodesAndSeparators) => 
			this.NodesAndSeparators = nodesAndSeparators;

		public SyntaxToken GetSeparator(int index)
		{
			if(index < 0 || index >= this.Count - 1)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			return (SyntaxToken)this.NodesAndSeparators[(index * 2) + 1];
		}

		public IEnumerator<T> GetEnumerator()
		{
			for(var i = 0; i < this.Count; i++)
			{
				yield return this[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public T this[int index] => (T)this.NodesAndSeparators[index * 2];
		public ImmutableArray<SyntaxNode> GetAllNodes() => this.NodesAndSeparators;
		public int Count => (this.NodesAndSeparators.Length + 1) / 2;
		private ImmutableArray<SyntaxNode> NodesAndSeparators { get; }
	}
}