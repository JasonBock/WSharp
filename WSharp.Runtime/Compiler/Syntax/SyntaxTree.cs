using System.Collections.Generic;
using System.Collections.Immutable;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class SyntaxTree
	{
		public SyntaxTree(SourceText text, IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFIleToken) => 
			(this.Text, this.Diagnostics, this.Root, this.EndOfFileToken) = 
				(text, diagnostics.ToImmutableArray(), root, endOfFIleToken);

		public static SyntaxTree Parse(string text) =>
			SyntaxTree.Parse(SourceText.From(text));

		public static SyntaxTree Parse(SourceText text) =>
			new Parser(text).Parse();

		public static IEnumerable<SyntaxToken> ParseTokens(string text) => 
			SyntaxTree.ParseTokens(SourceText.From(text));

		public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
		{
			var lexer = new Lexer(text);

			while (true)
			{
				var token = lexer.Lex();

				if (token.Kind != SyntaxKind.EndOfFileToken)
				{
					yield return token;
				}
				else
				{
					break;
				}
			}
		}

		public ImmutableArray<Diagnostic> Diagnostics { get; }
		public SyntaxToken EndOfFileToken { get; }
		public ExpressionSyntax Root { get; }
		public SourceText Text { get; }
	}
}