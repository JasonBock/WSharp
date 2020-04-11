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
			var lineStatements = this.ParseLineStatements();
			var endOfFileToken = this.Match(SyntaxKind.EndOfFileToken);
			return new CompilationUnitSyntax(lineStatements, endOfFileToken);
		}

		private LineStatementsSyntax ParseLineStatements()
		{
			var lines = new List<LineStatementSyntax>();
			var lineEnd = this.Text.Lines[^1].End;

			while (this.Current.Span.End < lineEnd)
			{
				lines.Add(this.ParseLineStatement());
			}

			return new LineStatementsSyntax(lines);
		}

		private LineStatementSyntax ParseLineStatement()
		{
			var lineNumber = this.ParsePrimaryExpression();
			var lines = this.ParseLineExpressionStatements();
			return new LineStatementSyntax(new ExpressionStatementSyntax(lineNumber), lines);
		}

		private List<ExpressionStatementSyntax> ParseLineExpressionStatements()
		{
			var semiColonFound = false;
			var lineStatements = new List<ExpressionStatementSyntax>();
			var lineIndex = this.Text.GetLineIndex(this.Current.Position);

			while(this.Text.GetLineIndex(this.Current.Position) == lineIndex)
			{
				var startToken = this.Current;

				if (!semiColonFound)
				{
					if(this.Peek(0).Kind == SyntaxKind.IdentifierToken)
					{
						lineStatements.Add(new ExpressionStatementSyntax(this.ParseCallExpression()));
					}
					else
					{
						var lineNumber = this.ParseBinaryExpression();

						var nextToken = this.Peek(0);

						if (nextToken.Kind != SyntaxKind.UpdateLineCountToken)
						{
							lineStatements.Add(new ExpressionStatementSyntax(new UnaryUpdateLineCountExpressionSyntax(lineNumber)));
						}
						else
						{
							var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
							var updateLineCountToken = this.ParseBinaryExpression();
							lineStatements.Add(new ExpressionStatementSyntax(new UpdateLineCountExpressionSyntax(
								lineNumber, operatorToken, updateLineCountToken)));
						}
					}
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
				else if (next.Kind == SyntaxKind.EndOfFileToken ||
					this.Text.GetLineIndex(next.Position) != lineIndex)
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

		private ExpressionSyntax ParsePrimaryExpression() =>
			this.Current.Kind switch
			{
				SyntaxKind.OpenParenthesisToken => this.ParseParenthesizedExpression(),
				SyntaxKind.TrueKeyword => this.ParseBooleanLiteralExpression(),
				SyntaxKind.FalseKeyword => this.ParseBooleanLiteralExpression(),
				SyntaxKind.NumberToken => this.ParseNumberLiteralExpression(),
				SyntaxKind.StringToken => this.ParseStringLiteralExpression(),
				_ => this.ParseCallExpression()
			};

		private ExpressionSyntax ParseCallExpression()
		{
			var identifier = this.Match(SyntaxKind.IdentifierToken);
			var openParenthesisToken = this.Match(SyntaxKind.OpenParenthesisToken);
			var arguments = this.ParseArguments();
			var closeParenthesisToken = this.Match(SyntaxKind.CloseParenthesisToken);

			return new CallExpressionSyntax(identifier, openParenthesisToken, arguments, closeParenthesisToken);
		}

		private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
		{
			var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

			while(this.Current.Kind != SyntaxKind.CloseParenthesisToken &&
				this.Current.Kind != SyntaxKind.EndOfFileToken)
			{
				var expression = this.ParseBinaryExpression();
				nodesAndSeparators.Add(expression);

				if(this.Current.Kind != SyntaxKind.CloseParenthesisToken)
				{
					var comma = this.Match(SyntaxKind.CommaToken);
					nodesAndSeparators.Add(comma);
				}
			}

			return new SeparatedSyntaxList<ExpressionSyntax>(nodesAndSeparators.ToImmutable());
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

		private ExpressionSyntax ParseStringLiteralExpression() =>
			new LiteralExpressionSyntax(this.Match(SyntaxKind.StringToken));

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