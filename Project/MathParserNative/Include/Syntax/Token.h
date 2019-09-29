// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef TOKEN_H
#define TOKEN_H

#include <string>
#include <map>

namespace Zorvan::Framework::MathParser::Syntax
{
	class Token
	{
	public:
		enum class Types
		{
			Number = 0,
			Parameter,

			Function,
			Equal,
			Greater,
			Less,
			GreaterEqual,
			LessEqual,

			Comma,

			OpenParentheses,
			CloseParentheses,

			Addition,
			Subtraction,
			Multiplication,
			Division,
			Remainder,
			Power,

			EndOfFile,
			Unknown
		};

	public:
		Token(Types Type, int LineNumber, int ColumnNumber, const std::string &Value) :
			m_Type(Type),
			m_LineNumber(LineNumber),
			m_ColumnNumber(ColumnNumber),
			m_Value(Value)
		{
		}

		inline Types GetType(void) const
		{
			return m_Type;
		}

		inline std::string GetTypeName(void) const
		{
			switch (m_Type)
			{
			case Token::Types::Number: return "Number";
			case Token::Types::Parameter: return "Parameter";
			case Token::Types::Function: return "Function";
			case Token::Types::Equal: return "Equal";
			case Token::Types::Greater: return "Greater";
			case Token::Types::Less: return "Less";
			case Token::Types::GreaterEqual: return "GreaterEqual";
			case Token::Types::LessEqual: return "LessEqual";
			case Token::Types::Comma: return "Comma";
			case Token::Types::OpenParentheses: return "OpenParentheses";
			case Token::Types::CloseParentheses: return "CloseParentheses";
			case Token::Types::Addition: return "Addition";
			case Token::Types::Subtraction: return "Subtraction";
			case Token::Types::Multiplication: return "Multiplication";
			case Token::Types::Division: return "Division";
			case Token::Types::Remainder: return "Remainder";
			case Token::Types::Power: return "Power";
			case Token::Types::EndOfFile: return "EndOfFile";
			}

			return "Unknown";
		}

		inline int GetLineNumber(void) const
		{
			return m_LineNumber;
		}

		inline int GetColumnNumber(void) const
		{
			return m_ColumnNumber;
		}

		inline const std::string &GetValue(void) const
		{
			return m_Value;
		}

	private:
		Types m_Type;
		int m_LineNumber;
		int m_ColumnNumber;
		std::string m_Value;
	};
}

#endif