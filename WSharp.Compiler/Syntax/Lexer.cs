using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using WSharp.Compiler.Text;

namespace WSharp.Compiler.Syntax;

internal sealed class Lexer
{
	private SyntaxKind kind;
	private int position;
	private int start;
	private readonly SourceText text;
	private readonly SyntaxTree tree;
	private readonly ImmutableArray<SyntaxTrivia>.Builder triviaBuilder =
		ImmutableArray.CreateBuilder<SyntaxTrivia>();
	private object? value;

	internal Lexer(SyntaxTree tree) =>
		(this.tree, this.text) = (tree ?? throw new ArgumentNullException(nameof(tree)), tree.Text);

	internal SyntaxToken Lex()
	{
		this.ReadTrivia(true);
		var leadingTrivia = this.triviaBuilder.ToImmutable();
		var tokenStart = this.position;
		this.ReadToken();
		var tokenKind = this.kind;
		var tokenValue = this.value;
		var tokenLength = this.position - this.start;
		this.ReadTrivia(false);
		var trailingTrivia = this.triviaBuilder.ToImmutable();

		var tokenText = SyntaxFacts.GetText(tokenKind);

		if (string.IsNullOrWhiteSpace(tokenText))
		{
			tokenText = this.text.ToString(tokenStart, tokenLength);
		}

		return new SyntaxToken(this.tree, tokenKind, tokenStart, tokenText, tokenValue,
			leadingTrivia, trailingTrivia);
	}

	private void ReadToken()
	{
		this.start = this.position;
		this.kind = SyntaxKind.BadToken;
		this.value = null;

		switch (this.Current)
		{
			case '\0':
				this.kind = SyntaxKind.EndOfFileToken;
				break;
			case '#':
				this.kind = SyntaxKind.UpdateLineCountToken;
				this.position++;
				break;
			case ',':
				this.kind = SyntaxKind.CommaToken;
				this.position++;
				break;
			case '+':
				this.kind = SyntaxKind.PlusToken;
				this.position++;
				break;
			case '-':
				this.kind = SyntaxKind.MinusToken;
				this.position++;
				break;
			case '*':
				this.kind = SyntaxKind.StarToken;
				this.position++;
				break;
			case '/':
				this.kind = SyntaxKind.SlashToken;
				this.position++;
				break;
			case '%':
				this.kind = SyntaxKind.PercentToken;
				this.position++;
				break;
			case '(':
				this.kind = SyntaxKind.OpenParenthesisToken;
				this.position++;
				break;
			case ')':
				this.kind = SyntaxKind.CloseParenthesisToken;
				this.position++;
				break;
			case ';':
				this.kind = SyntaxKind.SemicolonToken;
				this.position++;
				break;
			case '~':
				this.kind = SyntaxKind.TildeToken;
				this.position++;
				break;
			case '^':
				this.kind = SyntaxKind.HatToken;
				this.position++;
				break;
// Note: this disable is here because, even though the switch looks at this.Current,
// changing this.position can make this.Current change.
#pragma warning disable CA1508 // Avoid dead conditional code
			case '&':
				this.position++;
				if (this.Current == '&')
				{
					this.kind = SyntaxKind.AmpersandAmpersandToken;
					this.position++;
				}
				else
				{
					this.kind = SyntaxKind.AmpersandToken;
				}
				break;
			case '|':
				this.position++;
				if (this.Current == '|')
				{
					this.kind = SyntaxKind.PipePipeToken;
					this.position++;
				}
				else
				{
					this.kind = SyntaxKind.PipeToken;
				}
				break;
			case '=':
				this.position++;
				if (this.Current == '=')
				{
					this.kind = SyntaxKind.EqualsEqualsToken;
					this.position++;
				}
				break;
			case '!':
				this.position++;
				if (this.Current == '=')
				{
					this.kind = SyntaxKind.BangEqualsToken;
					this.position++;
				}
				else
				{
					this.kind = SyntaxKind.BangToken;
				}
				break;
			case '<':
				this.position++;
				if (this.Current == '=')
				{
					this.kind = SyntaxKind.LessOrEqualsToken;
					this.position++;
				}
				else
				{
					this.kind = SyntaxKind.LessToken;
				}
				break;
			case '>':
				this.position++;
				if (this.Current == '=')
				{
					this.kind = SyntaxKind.GreaterOrEqualsToken;
					this.position++;
				}
				else
				{
					this.kind = SyntaxKind.GreaterToken;
				}
				break;
#pragma warning restore CA1508 // Avoid dead conditional code
			case '"':
				this.ReadString();
				break;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				this.ReadNumberToken();
				break;
			default:
				if (char.IsLetter(this.Current))
				{
					this.ReadIdentifierOrKeyword();
				}
				else
				{
					this.Diagnostics.ReportBadCharacter(
						new TextLocation(this.text, new TextSpan(this.position, 1)), this.Current);
					this.position++;
				}

				break;
		}
	}

	private void ReadTrivia(bool isLeading)
	{
		this.triviaBuilder.Clear();
		var done = false;

		while (!done)
		{
			this.start = this.position;
			this.kind = SyntaxKind.BadToken;
			this.value = null;

			switch (this.Current)
			{
				case '\0':
					done = true;
					break;
				case '/':
					if (this.Lookahead == '/')
					{
						this.ReadSingleLineComment();
					}
					else if (this.Lookahead == '*')
					{
						this.ReadMultiLineComment();
					}
					else
					{
						done = true;
					}

					break;
				case '\n':
				case '\r':
					if (!isLeading)
					{
						done = true;
					}
					this.ReadLineBreak();
					break;
				case ' ':
				case '\t':
					this.ReadWhitespace();
					break;
				default:
					if (char.IsWhiteSpace(this.Current))
					{
						this.ReadWhitespace();
					}
					else
					{
						done = true;
					}
					break;
			}

			var length = this.position - this.start;

			if (length > 0)
			{
				var text = this.text.ToString(this.start, length);
				this.triviaBuilder.Add(new SyntaxTrivia(this.tree, this.kind, this.start, text));
			}
		}
	}

	private void ReadSingleLineComment()
	{
		this.position += 2;
		var done = false;

		while (!done)
		{
			switch (this.Current)
			{
				case '\r':
				case '\n':
				case '\0':
					done = true;
					break;
				default:
					this.position++;
					break;
			}
		}

		this.kind = SyntaxKind.SingleLineCommentTrivia;
	}

	private void ReadMultiLineComment()
	{
		this.position += 2;
		var done = false;

		while (!done)
		{
			switch (this.Current)
			{
				case '\0':
					this.Diagnostics.ReportUnterminatedMultiLineComment(
						new TextLocation(this.text, new TextSpan(this.start, 2)));
					done = true;
					break;
				case '*':
					if (this.Lookahead == '/')
					{
						done = true;
						this.position++;
					}

					this.position++;
					break;
				default:
					this.position++;
					break;
			}
		}

		this.kind = SyntaxKind.MultiLineCommentTrivia;
	}

	private void ReadString()
	{
		this.position++;
		var builder = new StringBuilder();
		var done = false;

		while (!done)
		{
			switch (this.Current)
			{
				case '\0':
				case '\r':
				case '\n':
					this.Diagnostics.ReportUnterminatedString(
						new TextLocation(this.text, new TextSpan(this.start, 1)));
					done = true;
					break;
				case '"':
					if (this.Lookahead == '"')
					{
						builder.Append(this.Current);
						this.position += 2;
					}
					else
					{
						this.position++;
						done = true;
					}
					break;
				default:
					builder.Append(this.Current);
					this.position++;
					break;
			}
		}

		this.kind = SyntaxKind.StringToken;
		this.value = builder.ToString();
	}

	private void ReadIdentifierOrKeyword()
	{
		while (char.IsLetter(this.Current))
		{
			this.position++;
		}

		var length = this.position - this.start;
		var text = this.text.ToString(this.start, length);
		this.kind = SyntaxFacts.GetKeywordKind(text);
	}

	private void ReadWhitespace()
	{
		var done = false;

		while (!done)
		{
			switch (this.Current)
			{
				case '\0':
				case '\r':
				case '\n':
					done = true;
					break;
				default:
					{
						if (!char.IsWhiteSpace(this.Current))
						{
							done = true;
						}
						else
						{
							this.position++;
						}
						break;
					}
			}
		}

		this.kind = SyntaxKind.WhitespaceTrivia;
	}

	private void ReadLineBreak()
	{
		if (this.Current == '\r' && this.Lookahead == '\n')
		{
			this.position += 2;
		}
		else
		{
			this.position++;
		}

		this.kind = SyntaxKind.LineBreakTrivia;
	}

	private void ReadNumberToken()
	{
		while (char.IsDigit(this.Current))
		{
			this.position++;
		}

		var length = this.position - this.start;
		var text = this.text.ToString(this.start, length);

		if (!BigInteger.TryParse(text, out var value))
		{
			this.Diagnostics.ReportInvalidNumber(
				new TextLocation(this.text, new TextSpan(this.start, length)), text);
		}

		this.kind = SyntaxKind.NumberToken;
		this.value = value;
	}

	private char Peek(int offset)
	{
		var index = this.position + offset;

		if (index >= this.text.Length)
		{
			return '\0';
		}

		return this.text[index];
	}

	private char Current => this.Peek(0);
	private char Lookahead => this.Peek(1);
	public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();
}
