﻿using System;
using System.Collections;
using System.Collections.Generic;
using WSharp.Runtime.Compiler.Symbols;
using WSharp.Runtime.Compiler.Syntax;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler
{
	public sealed class DiagnosticBag
		: IEnumerable<Diagnostic>
	{
		private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

		public IEnumerator<Diagnostic> GetEnumerator() => this.diagnostics.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public void AddRange(DiagnosticBag diagnostics) =>
			this.diagnostics.AddRange(diagnostics);

		private void Report(TextSpan span, string message) =>
			this.diagnostics.Add(new Diagnostic(span, message));

		public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type) =>
			this.Report(span, $"The number '{text}' isn't a valid '{type}'.");

		public void ReportUnterminatedString(TextSpan span) =>
			this.Report(span, $"Unterminated string literal.");

		public void ReportBadCharacter(int position, char character) => 
			this.Report(new TextSpan(position, 1), $"Bad character input: '{character}'.");

		public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind) => 
			this.Report(span, $"Unexpected token <{actualKind}>, expected <{expectedKind}>.");

		public void ReportExpressionMustHaveValue(TextSpan span) =>
			this.Report(span, "Expression must have a value.");

		public void ReportMissingSemicolon(TextSpan span) => 
			this.Report(span, "; expected.");

		public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType) =>
			this.Report(span, $"Unary operator '{operatorText}' is not defined for type '{operandType}'.");

		public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType) => 
			this.Report(span, $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.");

		internal void ReportMissingLineStatements(TextSpan span) =>
			this.Report(span, "No line statements exist.");

		public void ReportUndefinedLineCountOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType) => 
			this.Report(span, $"Update line count operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.");

		public void ReportWrongArgumentCount(TextSpan span, string name, int expectedCount, int actualCount) =>
			this.Report(span, $"Function '{name}' requires {expectedCount} argument(s) but was given {actualCount}.");

		public void ReportUndefinedFunction(SyntaxToken identifier) =>
			this.Report(identifier.Span, $"Function '{identifier.Text}' doesn't exist.");

		internal void ReportWrongArgumentType(TextSpan span, string name, TypeSymbol expectedType, TypeSymbol actualType) =>
			this.Report(span, $"Parameter '{name}' requires a value of type {expectedType} but was given a value of type {actualType}.");

		internal void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType) =>
			this.Report(span, $"Cannot convert type '{fromType}' to '{toType}'.");

		internal void ReportUnexpectedArgumentSyntax(TextSpan span) =>
			this.Report(span, $"Unexpected argument syntax.");

		internal void ReportUnexpectedLineStatementToken(TextSpan span) =>
			this.Report(span, $"Unexpected line statement token.");
	}
}