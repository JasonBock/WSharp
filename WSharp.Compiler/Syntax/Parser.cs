using System.Collections.Immutable;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax;

public sealed class Parser
{
	private int position;
	private readonly ImmutableArray<SyntaxToken> tokens;

	public Parser(SyntaxTree tree)
	{
		ArgumentNullException.ThrowIfNull(tree);

		var tokens = new List<SyntaxToken>();
		var lexer = new Lexer(tree);
		var badTokens = new List<SyntaxToken>();

		SyntaxToken token;

		do
		{
			token = lexer.Lex();

			if (token.Kind == SyntaxKind.BadToken)
			{
				badTokens.Add(token);
			}
			else
			{
				if (badTokens.Count > 0)
				{
					var leadingTrivia = token.LeadingTrivia.ToBuilder();
					var index = 0;

					foreach (var badToken in badTokens)
					{
						foreach (var leading in badToken.LeadingTrivia)
						{
							leadingTrivia.Insert(index++, leading);
						}

						var trivia = new SyntaxTrivia(tree, SyntaxKind.SkippedTextTrivia, badToken.Position, badToken.Text);
						leadingTrivia.Insert(index++, trivia);

						foreach (var trailing in badToken.TrailingTrivia)
						{
							leadingTrivia.Insert(index++, trailing);
						}
					}

					badTokens.Clear();
					token = new SyntaxToken(token.Tree, token.Kind, token.Position, token.Text, token.Value,
						leadingTrivia.ToImmutable(), token.TrailingTrivia);
				}

				tokens.Add(token);
			}
		} while (token.Kind != SyntaxKind.EndOfFileToken);

		this.Tree = tree;
		this.Text = tree.Text;
		this.tokens = tokens.ToImmutableArray();
		this.Diagnostics.AddRange(lexer.Diagnostics);
	}

	private SyntaxToken Match(SyntaxKind kind)
	{
		if (this.Current.Kind == kind)
		{
			return this.Next();
		}

		this.Diagnostics.ReportUnexpectedToken(this.Current.Location, this.Current.Kind, kind);
		return new SyntaxToken(this.Tree, kind, this.Current.Position, string.Empty, null,
			ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);
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
		return new CompilationUnitSyntax(this.Tree, lineStatements, endOfFileToken);
	}

	private LineStatementsSyntax ParseLineStatements()
	{
		var lines = new List<LineStatementSyntax>();
		var lineEnd = this.Text.Lines[^1].End;

		while (this.Current.Span.End < lineEnd)
		{
			lines.Add(this.ParseLineStatement());
		}

		return new LineStatementsSyntax(this.Tree, lines);
	}

	private LineStatementSyntax ParseLineStatement()
	{
		var currentDiagnosticCount = this.Diagnostics.Count;
		var lineNumber = this.ParseNumberLiteralExpression();

		if (this.Diagnostics.Count > currentDiagnosticCount)
		{
			this.position++;
		}

		var lineStatements = this.ParseLineExpressionStatements();

		if (lineStatements.Count == 0)
		{
			this.Diagnostics.ReportMissingLineStatements(lineNumber.Location);
		}

		return new LineStatementSyntax(this.Tree, new ExpressionStatementSyntax(this.Tree, lineNumber), lineStatements);
	}

	private List<ExpressionStatementSyntax> ParseLineExpressionStatements()
	{
		var semiColonFound = false;
		var lineStatements = new List<ExpressionStatementSyntax>();
		var lineIndex = this.Text.GetLineIndex(this.Current.Position);

		while (this.Text.GetLineIndex(this.Current.Position) == lineIndex)
		{
			var startToken = this.Current;

			if (!semiColonFound)
			{
				if (this.Peek(0).Kind == SyntaxKind.IdentifierToken)
				{
					lineStatements.Add(new ExpressionStatementSyntax(this.Tree, this.ParseCallExpression()));
				}
				else if (this.Peek(0).Kind == SyntaxKind.NumberToken ||
					(this.Peek(0).Kind == SyntaxKind.MinusToken && this.Peek(1).Kind == SyntaxKind.NumberToken))
				{
					var lineNumber = this.ParseBinaryExpression();
					var nextToken = this.Peek(0);

					if (nextToken.Kind != SyntaxKind.UpdateLineCountToken)
					{
						lineStatements.Add(new ExpressionStatementSyntax(
							this.Tree, new UnaryUpdateLineCountExpressionSyntax(this.Tree, lineNumber)));
					}
					else
					{
						var operatorToken = this.Match(SyntaxKind.UpdateLineCountToken);
						var updateLineCountToken = this.ParseBinaryExpression();
						lineStatements.Add(new ExpressionStatementSyntax(
							this.Tree, new UpdateLineCountExpressionSyntax(
								this.Tree, lineNumber, operatorToken, updateLineCountToken)));
					}
				}
				else
				{
					this.Diagnostics.ReportUnexpectedLineStatementToken(startToken.Location);
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
					this.Diagnostics.ReportMissingSemicolon(new TextLocation(this.Text, new TextSpan(this.Current.Span.Start - 1, 1)));
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

		return new CallExpressionSyntax(this.Tree, identifier, openParenthesisToken, arguments, closeParenthesisToken);
	}

	private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
	{
		var nodesAndSeparators = ImmutableArray.CreateBuilder<SyntaxNode>();

		var parseNextArgument = true;

		while (parseNextArgument &&
			this.Current.Kind != SyntaxKind.CloseParenthesisToken &&
			this.Current.Kind != SyntaxKind.EndOfFileToken)
		{
			var expression = this.ParseBinaryExpression();
			nodesAndSeparators.Add(expression);

			if (this.Current.Kind == SyntaxKind.CommaToken)
			{
				var comma = this.Match(SyntaxKind.CommaToken);
				nodesAndSeparators.Add(comma);
			}
			else
			{
				parseNextArgument = false;
			}
		}

		if (nodesAndSeparators.Count > 0 && nodesAndSeparators.Count % 2 == 0)
		{
			this.Diagnostics.ReportUnexpectedArgumentSyntax(this.Current.Location);
		}

		return new SeparatedSyntaxList<ExpressionSyntax>(nodesAndSeparators.ToImmutable());
	}

	private ExpressionSyntax ParseNumberLiteralExpression()
	{
		var numberToken = this.Match(SyntaxKind.NumberToken);
		return new LiteralExpressionSyntax(this.Tree, numberToken);
	}

	private ExpressionSyntax ParseParenthesizedExpression()
	{
		var left = this.Match(SyntaxKind.OpenParenthesisToken);
		var expression = this.ParseBinaryExpression();
		var right = this.Match(SyntaxKind.CloseParenthesisToken);
		return new ParenthesizedExpressionSyntax(this.Tree, left, expression, right);
	}

	private ExpressionSyntax ParseBooleanLiteralExpression()
	{
		var isTrue = this.Current.Kind == SyntaxKind.TrueKeyword;
		var keywordToken = isTrue ?
			this.Match(SyntaxKind.TrueKeyword) : this.Match(SyntaxKind.FalseKeyword);
		return new LiteralExpressionSyntax(this.Tree, keywordToken, isTrue);
	}

	private ExpressionSyntax ParseStringLiteralExpression() =>
		new LiteralExpressionSyntax(this.Tree, this.Match(SyntaxKind.StringToken));

	private ExpressionSyntax ParseBinaryExpression(int parentPrecendence = 0)
	{
		ExpressionSyntax left;

		var unaryOperatorPrecedence = this.Current.Kind.GetUnaryOperatorPrecedence();

		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecendence)
		{
			var operatorToken = this.Next();
			var operand = this.ParseBinaryExpression(unaryOperatorPrecedence);
			left = new UnaryExpressionSyntax(this.Tree, operatorToken, operand);
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
			left = new BinaryExpressionSyntax(this.Tree, left, operatorToken, right);
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
	public SyntaxTree Tree { get; }
}