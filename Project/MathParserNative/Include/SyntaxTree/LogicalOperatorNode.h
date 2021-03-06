// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef LOGICALOPERATORNODE_H
#define LOGICALOPERATORNODE_H

#include "..\Common.h"
#include "..\SyntaxTree\OperatorNode.h"

namespace GameFramework::MathParser::SyntaxTree
{
	class MATH_PARSER_API LogicalOperatorNode : public OperatorNode
	{
	public:
		enum class Operators
		{
			Equal,
			Greater,
			Less,
			GreaterEqual,
			LessEqual,
			Unknown
		};

	public:
		LogicalOperatorNode(Operators Operator)
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
			case Operators::Equal: return "Equal";
			case Operators::Greater: return "Greater";
			case Operators::Less: return "Less";
			case Operators::GreaterEqual: return "GreaterEqual";
			case Operators::LessEqual: return "LessEqual";
			}

			return "Unknown";
		}

	private:
		Operators m_Operator;
	};
}

#endif