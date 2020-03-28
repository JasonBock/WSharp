using System;
using System.Collections.Generic;
using System.Numerics;

namespace WSharp.Runtime.Compiler.Syntax
{
	public sealed class Lexer
	{
		private readonly List<string> diagnostics = new List<string>();
		private int position;
		private readonly string text;

		public Lexer(string text) => 
			this.text = text ?? throw new ArgumentNullException(nameof(text));

		private char GetCurrent()
		{
			if (this.position >= this.text.Length)
			{
				return '\0';
			}

			return this.text[this.position];
		}

		public SyntaxToken Lex() 
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

				if(!BigInteger.TryParse(text, out var value))
				{
					this.diagnostics.Add($"The number {text} isn't a valid BigInteger.");
				}

				return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
			}

			if (char.IsWhiteSpace(this.GetCurrent()))
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

			switch (this.GetCurrent())
			{
				case '#':
					return new SyntaxToken(SyntaxKind.UpdateLineCountToken, this.position++, "#", null);
				case ',':
					return new SyntaxToken(SyntaxKind.CommaToken, this.position++, ",", null);
				case '+':
					return new SyntaxToken(SyntaxKind.PlusToken, this.position++, "+", null);
				case '-':
					return new SyntaxToken(SyntaxKind.MinusToken, this.position++, "-", null);
				case '*':
					return new SyntaxToken(SyntaxKind.StarToken, this.position++, "*", null);
				case '/':
					return new SyntaxToken(SyntaxKind.SlashToken, this.position++, "/", null);
				case '(':
					return new SyntaxToken(SyntaxKind.OpenParenthesisToken, this.position++, "(", null);
				case ')':
					return new SyntaxToken(SyntaxKind.CloseParenthesisToken, this.position++, "(", null);
				case ';':
					return new SyntaxToken(SyntaxKind.SemicolonToken, this.position++, ";", null);
			}

			this.diagnostics.Add($"ERROR: bad character input: '{this.GetCurrent()}'");
			return new SyntaxToken(SyntaxKind.BadToken, this.position++, this.text.Substring(this.position - 1, 1), null);
		}

		private void UpdatePosition() => this.position++;

		public IEnumerable<string> Diagnostics => this.diagnostics;
	}
}