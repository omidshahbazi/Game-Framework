// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef PARAMETERNODE_H
#define PARAMETERNODE_H

#include "..\SyntaxTree\TreeNode.h"
#include <string>

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class ParameterNode : public TreeNode
	{
	public:
		ParameterNode(const std::string &Name) :
			m_Name(Name)
		{
		}

		inline const std::string &GetName(void) const
		{
			return m_Name;
		}

	private:
		std::string m_Name;
	};
}

#endif