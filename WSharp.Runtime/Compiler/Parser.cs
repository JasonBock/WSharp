using System.Collections.Generic;

namespace WSharp.Runtime.Compiler
{
	public sealed class Parser
	{
		private readonly List<string> diagnostics = new List<string>();
		private int position;
		private readonly SyntaxToken[] tokens;

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

		private SyntaxToken Match(SyntaxKind kind)
		{
			if (this.Current.Kind == kind)
			{
				return this.Next();
			}

			this.diagnostics.Add($"Error: Unexpected token <{this.Current.Kind}>, expected <{kind}>");
			return new SyntaxToken(kind, this.Current.Position, string.Empty, null);
		}

		private SyntaxToken Next()
		{
			var current = this.Current;
			this.position++;
			return current;
		}

		public SyntaxTree Parse()
		{
			var lineNumber = this.Match(SyntaxKind.NumberToken);
			var lineExpressions = new LineExpressionSyntax(new NumberExpressionSyntax(lineNumber), this.ParseLineExpressions());
			var endOfFileToken = this.Match(SyntaxKind.EndOfFileToken);

			return new SyntaxTree(this.diagnostics, lineExpressions, endOfFileToken);
		}

		private ExpressionSyntax ParseExpression() => this.ParseTerm();

		private ExpressionSyntax ParseFactor()
		{
			var left = this.ParseNumberExpression();

			while (this.Current.Kind == SyntaxKind.StarToken ||
				this.Current.Kind == SyntaxKind.SlashToken)
			{
				var operatorToken = this.Next();
				var right = this.ParseNumberExpression();
				left = new BinaryExpressionSyntax(left, operatorToken, right);
			}

			return left;
		}

		private List<ExpressionSyntax> ParseLineExpressions()
		{
			var semiColonFound = false;
			var lineExpressions = new List<ExpressionSyntax>();

			while (this.position < this.tokens.Length)
			{
				var startToken = this.Current;

				if (!semiColonFound)
				{
					var lineNumber = this.ParseTerm();
					var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
					var updateLineCountToken = this.ParseTerm();
					lineExpressions.Add(new UpdateLineCountExpressionSyntax(
						lineNumber, operatorToken, updateLineCountToken));
				}

				var next = this.Peek(0);

				if (next.Kind == SyntaxKind.CommaToken)
				{
					this.position++;
				}
				else if (next.Kind == SyntaxKind.SemicolonToken)
				{
					semiColonFound = true;
					this.position++;
				}
				else if (next.Kind == SyntaxKind.EndOfFileToken)
				{
					if (!semiColonFound)
					{
						this.diagnostics.Add("Error: ; expected");
					}

					break;
				}

				if (this.Current == startToken)
				{
					this.Next();
				}
			}

			return lineExpressions;
		}

		private ExpressionSyntax ParseNumberExpression()
		{
			if (this.Current.Kind == SyntaxKind.OpenParenthesisToken)
			{
				var left = this.Next();
				var expression = this.ParseExpression();
				var right = this.Match(SyntaxKind.CloseParenthesisToken);
				return new ParenthesizedExpressionSyntax(left, expression, right);
			}

			var numberToken = this.Match(SyntaxKind.NumberToken);
			return new NumberExpressionSyntax(numberToken);
		}

		private ExpressionSyntax ParseTerm()
		{
			var left = this.ParseFactor();

			while (this.Current.Kind == SyntaxKind.PlusToken ||
				this.Current.Kind == SyntaxKind.MinusToken)
			{
				var operatorToken = this.Next();
				var right = this.ParseFactor();
				left = new BinaryExpressionSyntax(left, operatorToken, right);
			}

			return left;
		}

		private SyntaxToken Peek(int offset)
		{
			var index = this.position + offset;
			if (index >= this.tokens.Length)
			{
				return this.tokens[^1];
			}

			return this.tokens[index];
		}

		private SyntaxToken Current => this.Peek(0);

		public IEnumerable<string> Diagnostics => this.diagnostics;
	}
}