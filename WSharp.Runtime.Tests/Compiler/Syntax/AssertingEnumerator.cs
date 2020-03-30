using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Tests.Compiler.Syntax
{
	internal sealed class AssertingEnumerator
		: IDisposable
	{
		private readonly IEnumerator<SyntaxNode> enumerator;

		public AssertingEnumerator(SyntaxNode node) => 
			this.enumerator = AssertingEnumerator.Flatten(node).GetEnumerator();

		public void AssertNode(SyntaxKind kind)
		{
			Assert.That(this.enumerator.MoveNext(), Is.True);
			Assert.That(this.enumerator.Current.Kind, Is.EqualTo(kind));
			Assert.That(this.enumerator.Current, Is.Not.TypeOf<SyntaxToken>());
		}

		public void AssertToken(SyntaxKind kind, string text)
		{
			Assert.That(this.enumerator.MoveNext(), Is.True);
			Assert.That(this.enumerator.Current.Kind, Is.EqualTo(kind));
			var token = (SyntaxToken)this.enumerator.Current;
			Assert.That(token.Text, Is.EqualTo(text));
		}

		private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
		{
			var stack = new Stack<SyntaxNode>();
			stack.Push(node);

			while(stack.Count > 0)
			{
				var poppedNode = stack.Pop();
				yield return poppedNode;

				foreach(var poppedChildNode in poppedNode.GetChildren().Reverse())
				{
					stack.Push(poppedChildNode);
				}
			}
		}

		public void Dispose()
		{
			Assert.That(this.enumerator.MoveNext(), Is.False);
			this.enumerator.Dispose();
		}
	}
}
