using System.Collections.Generic;
using System.Linq;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class SyntaxTree
	{
		public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFIleToken) => 
			(this.Diagnostics, this.Root, this.EndOfFileToken) = (diagnostics.ToArray(), root, endOfFIleToken);

		public static SyntaxTree Parse(string text) => 
			new Parser(text).Parse();

		public static IEnumerable<SyntaxToken> ParseTokens(string text)
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

		public IReadOnlyList<Diagnostic> Diagnostics { get; }
		public SyntaxToken EndOfFileToken { get; }
		public ExpressionSyntax Root { get; }
	}
}