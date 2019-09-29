// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef NUMBERNODE_H
#define NUMBERNODE_H

#include "..\SyntaxTree\TreeNode.h"

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class NumberNode : public TreeNode
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