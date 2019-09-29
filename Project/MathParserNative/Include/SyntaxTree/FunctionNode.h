// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef FUNCTIONNODE_H
#define FUNCTIONNODE_H

#include "..\SyntaxTree\TreeNode.h"

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class FunctionNode : public TreeNode
	{
	public:
		FunctionNode(const std::string &Name) :
			m_Name(Name)
		{
		}

		inline const std::string &GetName(void) const
		{
			return m_Name;
		}

		inline const TreeNodeCollection &GetParameters(void) const
		{
			return m_Parameters;
		}

		inline void InsertParameter(int Index, TreeNode *Parameter)
		{
			auto it = m_Parameters.begin();

			for (int i = 0; i < Index; ++i)
				it++;

			m_Parameters.insert(it, Parameter);
		}

	private:
		std::string m_Name;
		TreeNodeCollection m_Parameters;
	};
}

#endif