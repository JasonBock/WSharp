using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Runtime.Compiler.Binding;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler
{
	public sealed class Compilation
	{
		public Compilation(SyntaxTree tree) => this.Tree = tree;

		public EvaluationResult Evaluate()
		{
			var binder = new Binder();
			var expression = binder.BindExpression(this.Tree.Root);

			var diagnostics = this.Tree.Diagnostics.Concat(binder.Diagnostics).ToArray();

			if(diagnostics.Length > 0)
			{
				return new EvaluationResult(diagnostics, ImmutableList<Line>.Empty);
			}

			var evaluator = new Evaluator();
			var lines = evaluator.Evaluate(new List<BoundExpression> { expression });

			return new EvaluationResult(Array.Empty<Diagnostic>(), lines);
		}

		public SyntaxTree Tree { get; }
	}
}