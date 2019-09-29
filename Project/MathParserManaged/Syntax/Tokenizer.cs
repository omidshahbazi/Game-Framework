// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.Syntax
{
	class Tokenizer
	{
		private char[] expr = null;
		private int index = 0;
		private int lineNumber = 1;
		private int columnNumber = 0;
		private Token currentToken = null;

		public Tokenizer(string Expr)
		{
			expr = Expr.ToCharArray();
		}

		public Token ReadNextToken()
		{
			char c;
			if (IsEOF())
				return CreateToken(Token.Types.EndOfFile, "");

			while (IsWhitespace(c = ReadNextChar()))
			{
				if (IsEOF())
					return CreateToken(Token.Types.EndOfFile, "");

				if (IsNewline(c))
				{
					++lineNumber;
					columnNumber = 0;
				}
			}

			if (c == '(')
				return CreateToken(Token.Types.OpenParentheses, c);
			else if (c == ')')
				return CreateToken(Token.Types.CloseParentheses, c);
			else if (c == '+')
				return CreateToken(Token.Types.Addition, c);
			else if (c == '-' && !IsNumber(GetNextChar()))
				return CreateToken(Token.Types.Subtraction, c);
			else if (c == '*')
				return CreateToken(Token.Types.Multiplication, c);
			else if (c == '/')
				return CreateToken(Token.Types.Division, c);
			else if (c == '%')
				return CreateToken(Token.Types.Remainder, c);
			else if (c == '^')
				return CreateToken(Token.Types.Power, c);

			else if (c == '=')
				return CreateToken(Token.Types.Equal, c);
			else if (c == '>')
			{
				if (GetNextChar() == '=')
				{
					ReadNextChar();

					return CreateToken(Token.Types.GreaterEqual, c);
				}

				return CreateToken(Token.Types.Greater, c);
			}
			else if (c == '<')
			{
				if (GetNextChar() == '=')
				{
					ReadNextChar();

					return CreateToken(Token.Types.LessEqual, c);
				}

				return CreateToken(Token.Types.LessEqual, c);
			}
			else if (c == ',')
				return CreateToken(Token.Types.Comma, c);

			else if (IsNumber(c) || c == '-')
			{
				string literal = ReadNumber(c);

				return CreateToken(Token.Types.Number, literal);
			}
			else if (IsAlphabetic(c))
			{
				string literal = ReadLiteral(c);

				if (SkipWhitespace())
					if (GetNextChar() == '(')
						return CreateToken(Token.Types.Function, literal);

				return CreateToken(Token.Types.Parameter, literal);
			}

			return CreateToken(Token.Types.Unknown, c);
		}

		public Token GetCurrentToken()
		{
			return currentToken;
		}

		private bool SkipWhitespace()
		{
			char c;
			if (IsEOF())
				return false;

			while (IsWhitespace(c = GetNextChar()))
			{
				if (IsEOF())
					break;

				ReadNextChar();

				if (IsNewline(c))
				{
					++lineNumber;
					columnNumber = 0;
				}
			}
			return true;
		}

		private char ReadNextChar()
		{
			++columnNumber;
			return expr[index++];
		}

		private char GetNextChar()
		{
			return expr[index];
		}

		private Token CreateToken(Token.Types Type, char Value)
		{
			return CreateToken(Type, Value.ToString());
		}

		private Token CreateToken(Token.Types Type, string Value)
		{
			int colNum = columnNumber - (Value.Length != 0 ? Value.Length - 1 : 0);

			currentToken = new Token(Type, lineNumber, colNum, Value);

			return currentToken;
		}

		private string ReadLiteral(char CurrentChar)
		{
			string literal = "";

			do
			{
				literal += CurrentChar;

				if (IsEOF())
					break;

				char nextChar = GetNextChar();

				if (!(IsAlphabetic(nextChar) || IsNumber(nextChar)) || IsWhitespace(nextChar))
					break;

				CurrentChar = ReadNextChar();
			} while (true);

			return literal;
		}

		private string ReadNumber(char CurrentChar)
		{
			string literal = "";

			do
			{
				literal += CurrentChar;

				if (IsEOF())
					break;

				char nextChar = GetNextChar();

				if (!IsNumber(nextChar))
					break;

				CurrentChar = ReadNextChar();
			} while (true);

			return literal;
		}

		private bool IsNumber(char Char)
		{
			return (char.IsDigit(Char) || Char == '.');
		}

		private bool IsAlphabetic(char Char)
		{
			return char.IsLetter(Char);
		}

		private bool IsNewline(char Char)
		{
			return (Char == '\n' || Char == '\r');
		}

		private bool IsWhitespace(char Char)
		{
			return (IsNewline(Char) || Char == ' ');
		}

		private bool IsEOF()
		{
			return (index == expr.Length);
		}
	}
}