﻿using System.Collections.Immutable;
using WSharp.Compiler.Binding;
using WSharp.Compiler.Emit;
using WSharp.Compiler.Syntax;
using WSharp.Runtime;

namespace WSharp.Compiler;

public sealed class Compilation
{
	public Compilation(SyntaxTree tree)
	{
		this.Tree = tree ?? throw new ArgumentNullException(nameof(tree));
		var binder = new Binder(this.Tree.Root);
		this.Statement = binder.CompilationUnit;
		this.Diagnostics = binder.Diagnostics;
	}

	public void EmitTree(TextWriter writer) =>
		this.Statement.WriteTo(writer);

	public EvaluationResult Evaluate()
	{
		var diagnostics = this.Tree.Diagnostics.Concat(this.Diagnostics).ToArray();

		if (diagnostics.Length > 0)
		{
			return new EvaluationResult(diagnostics.ToImmutableArray(), ImmutableArray<Line>.Empty);
		}

		var evaluator = new Evaluator(this.Statement);
		var lines = evaluator.Evaluate();

		return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, lines);
	}

	public EmitResult Emit(string moduleName, FileInfo[] references, FileInfo outputPath, bool emitDebugging = false) =>
		Emitter.Emit((BoundLineStatements)this.Statement, moduleName, references, outputPath, emitDebugging);

	public DiagnosticBag Diagnostics { get; }
	internal BoundStatement Statement { get; }
	public SyntaxTree Tree { get; }
}