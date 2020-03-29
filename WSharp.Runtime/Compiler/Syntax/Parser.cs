using System.Collections.Generic;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class Parser
	{
		private int position;
		private readonly SyntaxToken[] tokens;

		public Parser(string text)
		{
			var tokens = new List<SyntaxToken>();
			var lexer = new Lexer(text);

			SyntaxToken token;

			do
			{
				token = lexer.Lex();

				if (token.Kind != SyntaxKind.WhitespaceToken &&
					token.Kind != SyntaxKind.BadToken)
				{
					tokens.Add(token);
				}
			} while (token.Kind != SyntaxKind.EndOfFileToken);

			this.tokens = tokens.ToArray();
			this.Diagnostics.AddRange(lexer.Diagnostics);
		}

		private SyntaxToken Match(SyntaxKind kind)
		{
			if (this.Current.Kind == kind)
			{
				return this.Next();
			}

			this.Diagnostics.ReportUnexpectedToken(this.Current.Span, this.Current.Kind, kind);
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
			var lineExpressions = new LineExpressionSyntax(new LiteralExpressionSyntax(lineNumber), this.ParseLineExpressions());
			var endOfFileToken = this.Match(SyntaxKind.EndOfFileToken);

			return new SyntaxTree(this.Diagnostics, lineExpressions, endOfFileToken);
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
					// TODO: It is possible to have a line (see the Fibonacci program) like this:
					// 5 4,-3,7;
					// If a number isn't specified, then that line number is either increase or decreased based on the sign of the number.
					// So, if a Peek(1) for UpdateLineCountToken isn't found, but a comma, then we should short-circuit
					// and immediately create a UpdateLineCountExpressionSyntax. This may be odd because we don't have the "#" in this case,
					// so it may be more correct to have a "UnaryLineCountExpressionSyntax" node that just takes the line number.
					// From that, the binder can infer what to do from that point....maybe.
					var lineNumber = this.ParseBinaryExpression();
					var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
					var updateLineCountToken = this.ParseBinaryExpression();
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
						this.Diagnostics.ReportMissingSemicolon(new TextSpan(this.Current.Span.Start - 1, 1));
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

		private ExpressionSyntax ParsePrimaryExpression()
		{
			// TODO: Remove var declarations and then possibly switch to pattern matching.
			switch (this.Current.Kind)
			{
				case SyntaxKind.OpenParenthesisToken:
					var left = this.Next();
					var expression = this.ParseBinaryExpression();
					var right = this.Match(SyntaxKind.CloseParenthesisToken);
					return new ParenthesizedExpressionSyntax(left, expression, right);
				case SyntaxKind.TrueKeyword:
				case SyntaxKind.FalseKeyword:
					var keywordToken = this.Next();
					var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
					return new LiteralExpressionSyntax(keywordToken, value);
				case SyntaxKind.IdentifierToken:
					var identifierToken = this.Next();
					return new NameExpressionSyntax(identifierToken);
				default:
					var numberToken = this.Match(SyntaxKind.NumberToken);
					return new LiteralExpressionSyntax(numberToken);
			}
		}

		private ExpressionSyntax ParseBinaryExpression(int parentPrecendence = 0)
		{
			ExpressionSyntax left;

			var unaryOperatorPrecedence = this.Current.Kind.GetUnaryOperatorPrecedence();

			if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecendence)
			{
				var operatorToken = this.Next();
				var operand = this.ParseBinaryExpression(unaryOperatorPrecedence);
				left = new UnaryExpressionSyntax(operatorToken, operand);
			}
			else
			{
				left = this.ParsePrimaryExpression();
			}

			while (true)
			{
				var precedence = this.Current.Kind.GetBinaryOperatorPrecedence();

				if (precedence == 0 || precedence <= parentPrecendence)
				{
					break;
				}

				var operatorToken = this.Next();
				var right = this.ParseBinaryExpression(precedence);
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

		public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();
	}
}