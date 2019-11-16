// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef OPERATORNODE_H
#define OPERATORNODE_H

#include "..\Common.h"
#include "..\SyntaxTree\TreeNode.h"

namespace GameFramework::MathParser::SyntaxTree
{
	class MATH_PARSER_API OperatorNode : public TreeNode
	{
	public:
		inline TreeNode *GetLeftNode(void) const
		{
			return m_LeftNode;
		}

		inline TreeNode *GetRightNode(void) const
		{
			return m_RightNode;
		}

		inline void SetLeftNode(TreeNode *Node)
		{
			m_LeftNode = Node;
		}

		inline void SetRightNode(TreeNode *Node)
		{
			m_RightNode = Node;
		}

	private:
		TreeNode *m_LeftNode;
		TreeNode *m_RightNode;
	};
}

#endif