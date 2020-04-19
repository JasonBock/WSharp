﻿using System;
using System.Numerics;
using System.Text;
using WSharp.Runtime.Compiler.Symbols;
using WSharp.Runtime.Compiler.Text;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class Lexer
	{
		private SyntaxKind kind;
		private int position;
		private int start;
		private readonly SourceText text;
		private object? value;

		public Lexer(SourceText text) =>
			this.text = text ?? throw new ArgumentNullException(nameof(text));

		public SyntaxToken Lex()
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
				// TODO: Consider adding in a mod operator, %
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
				case ' ':
				case '\t':
				case '\n':
				case '\r':
					this.ReadWhitespace();
					break;
				default:
					if (char.IsLetter(this.Current))
					{
						this.ReadIdentifierOrKeyword();
					}
					else if (char.IsWhiteSpace(this.Current))
					{
						this.ReadWhitespace();
					}
					else
					{
						this.Diagnostics.ReportBadCharacter(this.position, this.Current);
						this.position++;
					}

					break;
			}

			var length = this.position - this.start;
			var text = SyntaxFacts.GetText(this.kind);

			if (string.IsNullOrWhiteSpace(text))
			{
				text = this.text.ToString(this.start, length);
			}

			return new SyntaxToken(this.kind, this.start, text, this.value);
		}

		private void ReadString()
		{
			this.position++;
			var builder = new StringBuilder();
			var done = false;

			while(!done)
			{
				switch(this.Current)
				{
					case '\0':
					case '\r':
					case '\n':
						var span = new TextSpan(this.start, 1);
						this.Diagnostics.ReportUnterminatedString(span);
						done = true;
						break;
					case '"':
						if(this.Lookahead == '"')
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
			while (char.IsWhiteSpace(this.Current))
			{
				this.position++;
			}

			this.kind = SyntaxKind.WhitespaceToken;
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
				this.Diagnostics.ReportInvalidNumber(new TextSpan(this.start, length), text, TypeSymbol.Integer);
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
}