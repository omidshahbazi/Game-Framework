// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TOKENIZER_H
#define TOKENIZER_H

#include "..\Common.h"
#include "..\Syntax\Token.h"
#include <string>

namespace GameFramework::MathParser::Syntax
{
	class MATH_PARSER_API Tokenizer
	{
	public:
		Tokenizer(const std::string &Expr);

		Token *ReadNextToken(void);

		Token *GetCurrentToken(void)
		{
			return m_CurrentToken;
		}

	private:
		inline bool SkipWhitespace()
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
					++m_LineNumber;
					m_ColumnNumber = 0;
				}
			}
			return true;
		}

		char ReadNextChar()
		{
			++m_ColumnNumber;
			return m_Expr[m_Index++];
		}

		char GetNextChar()
		{
			return m_Expr[m_Index];
		}

		inline Token *CreateToken(Token::Types Type, char Value)
		{
			return CreateToken(Type, std::string(1, Value));
		}

		inline Token *CreateToken(Token::Types Type, const std::string &Value)
		{
			int colNum = m_ColumnNumber - (Value.length() != 0 ? Value.length() - 1 : 0);

			m_CurrentToken = new Token(Type, m_LineNumber, colNum, Value);

			return m_CurrentToken;
		}

		std::string ReadLiteral(char CurrentChar);

		std::string ReadNumber(char CurrentChar);

		inline bool IsNumber(char Char)
		{
			return ((Char >= '0' && Char <= '9') || Char == '.');
		}

		inline bool IsAlphabetic(char Char)
		{
			return (Char >= 'A' && Char <= 'z');
		}

		inline bool IsNewline(char Char)
		{
			return (Char == '\n' || Char == '\r');
		}

		inline bool IsWhitespace(char Char)
		{
			return (IsNewline(Char) || Char == ' ');
		}

		inline bool IsEOF(void)
		{
			return (m_Index == m_Length);
		}

	private:
		const char *m_Expr;
		int m_Length = 0;
		int m_Index = 0;
		int m_LineNumber = 1;
		int m_ColumnNumber = 0;
		Token *m_CurrentToken;
	};
}

#endif