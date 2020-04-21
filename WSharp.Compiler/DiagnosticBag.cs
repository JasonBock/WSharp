using System;
using System.Collections;
using System.Collections.Generic;
using WSharp.Compiler.Symbols;
using WSharp.Compiler.Syntax;
using WSharp.Compiler.Text;

namespace WSharp.Compiler
{
	public sealed class DiagnosticBag
		: IEnumerable<Diagnostic>
	{
		private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

		public IEnumerator<Diagnostic> GetEnumerator() => this.diagnostics.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public void AddRange(DiagnosticBag diagnostics) =>
			this.diagnostics.AddRange(diagnostics);

		private void Report(TextLocation location, string message) =>
			this.diagnostics.Add(new Diagnostic(location, message));

		public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type) =>
			this.Report(location, $"The number '{text}' isn't a valid '{type}'.");

		public void ReportUnterminatedString(TextLocation location) =>
			this.Report(location, $"Unterminated string literal.");

		public void ReportBadCharacter(TextLocation location, char character) => 
			this.Report(location, $"Bad character input: '{character}'.");

		public void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind) => 
			this.Report(location, $"Unexpected token <{actualKind}>, expected <{expectedKind}>.");

		public void ReportExpressionMustHaveValue(TextLocation location) =>
			this.Report(location, "Expression must have a value.");

		public void ReportMissingSemicolon(TextLocation location) => 
			this.Report(location, "; expected.");

		public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandType) =>
			this.Report(location, $"Unary operator '{operatorText}' is not defined for type '{operandType}'.");

		public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType) => 
			this.Report(location, $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.");

		internal void ReportMissingLineStatements(TextLocation location) =>
			this.Report(location, "No line statements exist.");

		public void ReportUndefinedLineCountOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType) => 
			this.Report(location, $"Update line count operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.");

		public void ReportWrongArgumentCount(TextLocation location, string name, int expectedCount, int actualCount) =>
			this.Report(location, $"Function '{name}' requires {expectedCount} argument(s) but was given {actualCount}.");

		public void ReportUndefinedFunction(SyntaxToken identifier) =>
			this.Report(identifier.Location, $"Function '{identifier.Text}' doesn't exist.");

		internal void ReportWrongArgumentType(TextLocation location, string name, TypeSymbol expectedType, TypeSymbol actualType) =>
			this.Report(location, $"Parameter '{name}' requires a value of type {expectedType} but was given a value of type {actualType}.");

		internal void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType) =>
			this.Report(location, $"Cannot convert type '{fromType}' to '{toType}'.");

		internal void ReportUnexpectedArgumentSyntax(TextLocation location) =>
			this.Report(location, $"Unexpected argument syntax.");

		internal void ReportUnexpectedLineStatementToken(TextLocation location) =>
			this.Report(location, $"Unexpected line statement token.");

		internal void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol fromType, TypeSymbol toType) => 
			this.Report(location, $"Cannot convert type '{fromType}' to '{toType}'. An explicit conversion exists (are you missing a cast?)");
	}
}