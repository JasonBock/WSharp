using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

		public void ReportInvalidReference(FileInfo reference)
		{
			if (reference is null)
			{
				throw new ArgumentNullException(nameof(reference));
			}

			this.Report(default, $"The reference is not a valid .NET assembly: '{reference.FullName}'.");
		}

		public void ReportRequiredTypeNotFound(string wheneverName, string metadataName) => 
			this.Report(default,
				!string.IsNullOrWhiteSpace(wheneverName) ?
					$"The required type '{wheneverName}' ('{metadataName}') cannot be resolved.'" :
					$"The required type '{metadataName}' cannot be resolved.'");

		public void ReportRequiredTypeAmbiguous(string wheneverName, string metadataName, TypeDefinition[] foundTypes)
		{
			var assemblyNames = foundTypes.Select(_ => _.Module.Assembly.Name.Name);
			var assemblyNameList = string.Join(", ", assemblyNames);
			this.Report(default, 
				!string.IsNullOrWhiteSpace(wheneverName) ?
					$"The required type '{wheneverName}' ('{metadataName}') was found in multiple references: {assemblyNameList}." :
					$"The required type '{metadataName}' was found in multiple references: {assemblyNameList}.");
		}

		public void ReportRequiredMethodNotFound(string typeName, string methodName, string[] parameterTypeNames)
		{
			var parameterTypeNameList = string.Join(", ", parameterTypeNames);
			var message = $"The required method '{typeName}.{methodName}({parameterTypeNameList})' cannot be resolved among the given references.";
			this.Report(default, message);
		}

		public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type) =>
			this.Report(location, $"The number '{text}' isn't a valid '{type}'.");

		public void ReportUnterminatedString(TextLocation location) =>
			this.Report(location, $"Unterminated string literal.");
		
		public void ReportUnterminatedMultiLineComment(TextLocation location) =>
			this.Report(location, $"Unterminated multi-line comment.");

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

		public void ReportUndefinedFunction(SyntaxToken identifier)
		{
			if (identifier is null)
			{
				throw new ArgumentNullException(nameof(identifier));
			}

			this.Report(identifier.Location, $"Function '{identifier.Text}' doesn't exist.");
		}

		internal void ReportInvalidLineNumberReference(TextLocation location, BigInteger targetLineNumber) =>
			this.Report(location, $"The line number {targetLineNumber} does not exist.");

		internal void ReportWrongArgumentType(TextLocation location, string name, TypeSymbol expectedType, TypeSymbol actualType) =>
			this.Report(location, $"Parameter '{name}' requires a value of type {expectedType} but was given a value of type {actualType}.");

		internal void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType) =>
			this.Report(location, $"Cannot convert type '{fromType}' to '{toType}'.");

		internal void ReportUnexpectedArgumentSyntax(TextLocation location) =>
			this.Report(location, "Unexpected argument syntax.");

		internal void ReportUnexpectedLineStatementToken(TextLocation location) =>
			this.Report(location, "Unexpected line statement token.");

		internal void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol fromType, TypeSymbol toType) => 
			this.Report(location, $"Cannot convert type '{fromType}' to '{toType}'. An explicit conversion exists (are you missing a cast?)");

		internal void ReportNoStatementsAfterDefer(TextLocation location) =>
			this.Report(location, "No statements exist after a call to defer().");

		internal void ReportDuplicateLineNumber(TextLocation location, BigInteger lineNumber) =>
			this.Report(location, $"The line number {lineNumber} was already used.");

		public int Count => this.diagnostics.Count;
	}
}