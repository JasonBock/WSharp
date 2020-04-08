using System.Collections.Generic;
using System.Collections.Immutable;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class SyntaxTree
	{
		private SyntaxTree(SourceText text)
		{
			var parser = new Parser(text);
			var root = parser.ParseCompilationUnit();
			var diagnostics = parser.Diagnostics.ToImmutableArray();

			(this.Text, this.Diagnostics, this.Root) =
				(text, diagnostics.ToImmutableArray(), root);
		}

		public static SyntaxTree Parse(string text) =>
			SyntaxTree.Parse(SourceText.From(text));

		public static SyntaxTree Parse(SourceText text) =>
			new SyntaxTree(text);

		public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(string text) => 
			SyntaxTree.ParseTokens(SourceText.From(text));

		public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(SourceText text)
		{
			static IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
			{
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

			var lexer = new Lexer(text);
			var tokens = LexTokens(lexer).ToImmutableArray();
			return (tokens, lexer.Diagnostics.ToImmutableArray());
		}

		public ImmutableArray<Diagnostic> Diagnostics { get; }
		public CompilationUnitSyntax Root { get; }
		public SourceText Text { get; }
	}
}