// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef NUMBERNODE_H
#define NUMBERNODE_H

#include "..\Common.h"
#include "..\SyntaxTree\TreeNode.h"

namespace GameFramework::MathParser::SyntaxTree
{
	class MATH_PARSER_API NumberNode : public TreeNode
	{
	public:
		NumberNode(double Number) :
			m_Number(Number)
		{
		}

		inline const double &GetNumber(void) const
		{
			return m_Number;
		}

	private:
		double m_Number;
	};
}

#endif