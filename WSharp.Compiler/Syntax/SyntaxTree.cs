using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax
{
	public sealed class SyntaxTree
	{
		private delegate (CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) ParseHandler(SyntaxTree tree);

		private SyntaxTree(SourceText text, ParseHandler handler)
		{
			this.Text = text;
			(this.Root, this.Diagnostics) = handler(this);
		}

		public static async Task<SyntaxTree> LoadAsync(FileInfo file)
		{
			if (file is null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			return SyntaxTree.Parse(SourceText.From(
				await File.ReadAllTextAsync(file.FullName).ConfigureAwait(false), file));
		}

		public static SyntaxTree Parse(string text) =>
			SyntaxTree.Parse(SourceText.From(text));

		public static SyntaxTree Parse(SourceText text) =>
			new SyntaxTree(text, SyntaxTree.Parse);

		public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(string text) => 
			SyntaxTree.ParseTokens(SourceText.From(text));

		public static (ImmutableArray<SyntaxToken> tokens, ImmutableArray<Diagnostic> diagnostics) ParseTokens(SourceText text)
		{
			var tokens = new List<SyntaxToken>();

			(CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) ParseTokens(SyntaxTree tree)
			{
				var lexer = new Lexer(tree);

				while (true)
				{
					var token = lexer.Lex();

					if (token.Kind == SyntaxKind.EndOfFileToken)
					{
						return (new CompilationUnitSyntax(tree, new LineStatementsSyntax(tree, new List<LineStatementSyntax>()), token), 
							lexer.Diagnostics.ToImmutableArray());
					}

					tokens.Add(token);
				}
			}

			var tree = new SyntaxTree(text, ParseTokens);
			return (tokens.ToImmutableArray(), tree.Diagnostics); 
		}

		private static (CompilationUnitSyntax root, ImmutableArray<Diagnostic> diagnostics) Parse(SyntaxTree tree)
		{
			var parser = new Parser(tree);
			return (parser.ParseCompilationUnit(), parser.Diagnostics.ToImmutableArray());
		}

		public ImmutableArray<Diagnostic> Diagnostics { get; }
		public CompilationUnitSyntax Root { get; }
		public SourceText Text { get; }
	}
}