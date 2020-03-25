using System;
using System.Collections.Generic;
using System.Numerics;

namespace WSharp.Playground
{
	public sealed class Lexer
	{
		private readonly List<string> diagnostics = new List<string>();
		private int position;
		private readonly string text;

		public Lexer(string text) => 
			this.text = text ?? throw new ArgumentNullException(nameof(text));

		public SyntaxToken Next() 
		{
			if(this.position >= this.text.Length)
			{
				return new SyntaxToken(SyntaxKind.EndOfFileToken, this.position, "\0", null);
			}

			if(char.IsDigit(this.GetCurrent()))
			{
				var start = this.position;

				while(char.IsDigit(this.GetCurrent()))
				{
					this.UpdatePosition();
				}

				var length = this.position - start;
				var text = this.text.Substring(start, length);
				BigInteger.TryParse(text, out var value);

				return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
			}

			if(char.IsWhiteSpace(this.GetCurrent()))
			{
				var start = this.position;

				while (char.IsWhiteSpace(this.GetCurrent()))
				{
					this.UpdatePosition();
				}

				var length = this.position - start;
				var text = this.text.Substring(start, length);
				return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, text);
			}

			if(this.GetCurrent() == '#')
			{
				return new SyntaxToken(SyntaxKind.UpdateLineCountToken, this.position++, "#", null);
			}

			if (this.GetCurrent() == ',')
			{
				return new SyntaxToken(SyntaxKind.CommaToken, this.position++, ",", null);
			}

			/*
			if (this.GetCurrent() == '+')
			{
				return new SyntaxToken(SyntaxKind.PlusToken, this.position++, "+", null);
			}

			if (this.GetCurrent() == '-')
			{
				return new SyntaxToken(SyntaxKind.MinusToken, this.position++, "-", null);
			}

			if (this.GetCurrent() == '*')
			{
				return new SyntaxToken(SyntaxKind.StarToken, this.position++, "*", null);
			}

			if (this.GetCurrent() == '/')
			{
				return new SyntaxToken(SyntaxKind.SlashToken, this.position++, "/", null);
			}

			if (this.GetCurrent() == '(')
			{
				return new SyntaxToken(SyntaxKind.OpenParenthesisToken, this.position++, "(", null);
			}

			if (this.GetCurrent() == ')')
			{
				return new SyntaxToken(SyntaxKind.CloseParenthesisToken, this.position++, "(", null);
			}
			*/

			this.diagnostics.Add($"ERROR: bad character input: '{this.GetCurrent()}'");
			return new SyntaxToken(SyntaxKind.BadToken, this.position++, this.text.Substring(this.position - 1, 1), null);
		}

		private char GetCurrent()
		{
			if(this.position >= this.text.Length)
			{
				return '\0';
			}

			return this.text[this.position];
		}

		private void UpdatePosition() => this.position++;

		public IEnumerable<string> Diagnostics => this.diagnostics;
	}
}