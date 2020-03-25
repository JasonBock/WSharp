using System.Collections.Generic;

namespace WSharp.Playground
{
	public sealed class Parser
	{
		private readonly SyntaxToken[] tokens;
		private readonly List<string> diagnostics = new List<string>();
		private int position;

		public Parser(string text)
		{
			var tokens = new List<SyntaxToken>();
			var lexer = new Lexer(text);

			SyntaxToken token;

			do
			{
				token = lexer.Next();

				if (token.Kind != SyntaxKind.WhitespaceToken &&
					token.Kind != SyntaxKind.BadToken)
				{
					tokens.Add(token);
				}
			} while (token.Kind != SyntaxKind.EndOfFileToken);

			this.tokens = tokens.ToArray();
			this.diagnostics.AddRange(lexer.Diagnostics);
		}

		public IEnumerable<string> Diagnostics => this.diagnostics;

		private SyntaxToken Peek(int offset)
		{
			var index = this.position + offset;
			if (index >= this.tokens.Length)
			{
				return this.tokens[^1];
			}

			return this.tokens[index];
		}

		private SyntaxToken Next()
		{
			var current = this.Current;
			this.position++;
			return current;
		}

		private SyntaxToken Match(SyntaxKind kind)
		{
			if (this.Current.Kind == kind)
			{
				return this.Next();
			}

			this.diagnostics.Add($"Error: Unexpected token <{this.Current.Kind}, expected {kind}>");
			return new SyntaxToken(kind, this.Current.Position, string.Empty, null);
		}

		private List<ExpressionSyntax> ParseLineExpressions()
		{
			var lineExpressions = new List<ExpressionSyntax>();

			while (this.position < this.tokens.Length)
			{
				var lineNumber = this.Match(SyntaxKind.NumberToken);
				var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
				var updateLineCountToken = this.Match(SyntaxKind.NumberToken);
				lineExpressions.Add(new UpdateLineCountExpressionSyntax(
						new NumberExpressionSyntax(lineNumber), operatorToken, new NumberExpressionSyntax(updateLineCountToken)));

				var next = this.Peek(0);

				if (next.Kind == SyntaxKind.CommaToken)
				{
					// Skip
					this.position++;
				}
				else if (next.Kind == SyntaxKind.EndOfFileToken)
				{
					break;
				}
			}

			return lineExpressions;
		}

		public SyntaxTree Parse()
		{
			var expression = this.ParseExpression();
			var endOfFIleToken = this.Match(SyntaxKind.EndOfFileToken);

			return new SyntaxTree(this.diagnostics, expression, endOfFIleToken);
		}

		private ExpressionSyntax ParseExpression()
		{
			var lineNumber = this.Match(SyntaxKind.NumberToken);
			var lineExpressions = this.ParseLineExpressions();

			return new LineExpressionSyntax(new NumberExpressionSyntax(lineNumber), lineExpressions);
		}

		private SyntaxToken Current => this.Peek(0);
	}
}