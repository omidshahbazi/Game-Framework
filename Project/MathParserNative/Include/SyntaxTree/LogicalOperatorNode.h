// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef LOGICALOPERATORNODE_H
#define LOGICALOPERATORNODE_H

#include "..\SyntaxTree\OperatorNode.h"

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class LogicalOperatorNode : public OperatorNode
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