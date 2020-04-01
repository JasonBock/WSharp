using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class Parser
	{
		private int position;
		private readonly ImmutableArray<SyntaxToken> tokens;

		public Parser(SourceText text)
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

			this.Text = text;
			this.tokens = tokens.ToImmutableArray();
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

		public CompilationUnitSyntax ParseCompilationUnit()
		{
			var lineStatement = this.ParseLineStatement();
			var endOfFileToken = this.Match(SyntaxKind.EndOfFileToken);
			return new CompilationUnitSyntax(lineStatement, endOfFileToken);
		}

		private LineStatementSyntax ParseLineStatement()
		{
			var lineNumber = this.ParsePrimaryExpression();
			var lines = this.ParseLineStatements();
			return new LineStatementSyntax(new ExpressionStatementSyntax(lineNumber), lines);
		}

		private List<ExpressionStatementSyntax> ParseLineStatements()
		{
			var semiColonFound = false;
			var lineStatements = new List<ExpressionStatementSyntax>();

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

					// TODO: May want to move this into its own ParseLineStatement() 
					// as I'll eventually have to handle method invocations.
					var lineNumber = this.ParseBinaryExpression();
					var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
					var updateLineCountToken = this.ParseBinaryExpression();
					lineStatements.Add(new ExpressionStatementSyntax(new UpdateLineCountExpressionSyntax(
						lineNumber, operatorToken, updateLineCountToken)));
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

			return lineStatements;
		}

		private ExpressionSyntax ParsePrimaryExpression()
		{
			// TODO: Remove var declarations and then possibly switch to pattern matching.
			switch (this.Current.Kind)
			{
				case SyntaxKind.OpenParenthesisToken:
					return this.ParseParenthesizedExpression();
				case SyntaxKind.TrueKeyword:
				case SyntaxKind.FalseKeyword:
					return this.ParseBooleanLiteralExpression();
				case SyntaxKind.NumberToken:
					return this.ParseNumberLiteralExpression();
				case SyntaxKind.IdentifierToken:
				default:
					return this.ParseNameExpression();
			}
		}

		private ExpressionSyntax ParseNumberLiteralExpression()
		{
			var numberToken = this.Match(SyntaxKind.NumberToken);
			return new LiteralExpressionSyntax(numberToken);
		}

		private ExpressionSyntax ParseParenthesizedExpression()
		{
			var left = this.Match(SyntaxKind.OpenParenthesisToken);
			var expression = this.ParseBinaryExpression();
			var right = this.Match(SyntaxKind.CloseParenthesisToken);
			return new ParenthesizedExpressionSyntax(left, expression, right);
		}

		private ExpressionSyntax ParseBooleanLiteralExpression()
		{
			var isTrue = this.Current.Kind == SyntaxKind.TrueKeyword;
			var keywordToken = isTrue ? 
				this.Match(SyntaxKind.TrueKeyword) : this.Match(SyntaxKind.FalseKeyword);
			return new LiteralExpressionSyntax(keywordToken, isTrue);
		}

		private ExpressionSyntax ParseNameExpression()
		{
			var identifierToken = this.Match(SyntaxKind.IdentifierToken);
			return new NameExpressionSyntax(identifierToken);
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
		public SourceText Text { get; }
	}
}