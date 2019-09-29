// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#include "..\..\Include\Syntax\Tokenizer.h"
#include <sstream>

namespace Zorvan::Framework::MathParser::Syntax
{
	Tokenizer::Tokenizer(const std::string &Expr) :
		m_Expr(Expr.c_str()),
		m_Length(Expr.length()),
		m_Index(0),
		m_LineNumber(1),
		m_ColumnNumber(0),
		m_CurrentToken(nullptr)
	{
	}

	Token *Tokenizer::ReadNextToken(void)
	{
		char c;
		if (IsEOF())
			return CreateToken(Token::Types::EndOfFile, "");

		while (IsWhitespace(c = ReadNextChar()))
		{
			if (IsEOF())
				return CreateToken(Token::Types::EndOfFile, "");

			if (IsNewline(c))
			{
				++m_LineNumber;
				m_ColumnNumber = 0;
			}
		}

		if (c == '(')
			return CreateToken(Token::Types::OpenParentheses, c);
		else if (c == ')')
			return CreateToken(Token::Types::CloseParentheses, c);
		else if (c == '+')
			return CreateToken(Token::Types::Addition, c);
		else if (c == '-' && !IsNumber(GetNextChar()))
			return CreateToken(Token::Types::Subtraction, c);
		else if (c == '*')
			return CreateToken(Token::Types::Multiplication, c);
		else if (c == '/')
			return CreateToken(Token::Types::Division, c);
		else if (c == '%')
			return CreateToken(Token::Types::Remainder, c);
		else if (c == '^')
			return CreateToken(Token::Types::Power, c);

		else if (c == '=')
			return CreateToken(Token::Types::Equal, c);
		else if (c == '>')
		{
			if (GetNextChar() == '=')
			{
				ReadNextChar();

				return CreateToken(Token::Types::GreaterEqual, c);
			}

			return CreateToken(Token::Types::Greater, c);
		}
		else if (c == '<')
		{
			if (GetNextChar() == '=')
			{
				ReadNextChar();

				return CreateToken(Token::Types::LessEqual, c);
			}

			return CreateToken(Token::Types::LessEqual, c);
		}
		else if (c == ',')
			return CreateToken(Token::Types::Comma, c);
		else if (IsNumber(c) || c == '-')
		{
			std::string literal = ReadNumber(c);

			return CreateToken(Token::Types::Number, literal);
		}
		else if (IsAlphabetic(c))
		{
			std::string literal = ReadLiteral(c);

			if (SkipWhitespace())
				if (GetNextChar() == '(')
					return CreateToken(Token::Types::Function, literal);

			return CreateToken(Token::Types::Parameter, literal);
		}

		return CreateToken(Token::Types::Unknown, c);
	}

	std::string Tokenizer::ReadLiteral(char CurrentChar)
	{
		std::stringstream literal;

		do
		{
			literal << CurrentChar;

			if (IsEOF())
				break;

			char nextChar = GetNextChar();

			if (!(IsAlphabetic(nextChar) || IsNumber(nextChar)) || IsWhitespace(nextChar))
				break;

			CurrentChar = ReadNextChar();
		} while (true);

		return literal.str();
	}

	std::string Tokenizer::ReadNumber(char CurrentChar)
	{
		std::stringstream literal;

		do
		{
			literal << CurrentChar;

			if (IsEOF())
				break;

			char nextChar = GetNextChar();

			if (!IsNumber(nextChar))
				break;

			CurrentChar = ReadNextChar();
		} while (true);

		return literal.str();
	}
}
