using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using WSharp.Runtime.Compiler.Binding;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler
{
	public sealed class Compilation
	{
		public Compilation(SyntaxTree tree)
		{
			this.Tree = tree;
			var binder = new Binder();
			this.Statement = binder.BindCompilationUnit(this.Tree.Root);
			this.BinderDiagnostics = binder.Diagnostics;
		}

		public void EmitTree(TextWriter writer) => 
			this.Statement.WriteTo(writer);

		public EvaluationResult Evaluate()
		{
			var diagnostics = this.Tree.Diagnostics.Concat(this.BinderDiagnostics).ToArray();

			if(diagnostics.Length > 0)
			{
				return new EvaluationResult(diagnostics.ToImmutableArray(), ImmutableArray<Line>.Empty);
			}

			var evaluator = new Evaluator(this.Statement);
			var lines = evaluator.Evaluate();

			return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, lines);
		}

		private DiagnosticBag BinderDiagnostics { get; }
		public BoundStatement Statement { get; }
		public SyntaxTree Tree { get; }
	}
}