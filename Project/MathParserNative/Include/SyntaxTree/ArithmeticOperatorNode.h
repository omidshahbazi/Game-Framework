// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef ARITHMETICOPERATORNODE_H
#define ARITHMETICOPERATORNODE_H

#include "..\Common.h"
#include "..\SyntaxTree\OperatorNode.h"

namespace GameFramework::MathParser::SyntaxTree
{
	class MATH_PARSER_API ArithmeticOperatorNode : public OperatorNode
	{
	public:
		enum class Operators
		{
			Addition = 0,
			Subtraction,
			Multiplication,
			Division,
			Remainder,
			Power,
			Unknown
		};

	public:
		ArithmeticOperatorNode(Operators Operator)
		{
			m_Operator = Operator;
		}

		inline Operators GetOperator(void) const
		{
			return m_Operator;
		}

		inline std::string GetOperatorName(void) const
		{
			switch (m_Operator)
			{
			case Operators::Addition: return "Addition";
			case Operators::Subtraction: return "Subtraction";
			case Operators::Multiplication: return "Multiplication";
			case Operators::Division: return "Division";
			case Operators::Remainder: return "Remainder";
			case Operators::Power: return "Power";
			}

			return "Unknown";
		}

	private:
		Operators m_Operator;
	};
}

#endif