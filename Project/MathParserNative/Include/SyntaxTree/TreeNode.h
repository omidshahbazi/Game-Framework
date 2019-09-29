// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef TREENODE_H
#define TREENODE_H

#include <list>

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class TreeNode
	{
	public:
		virtual ~TreeNode(void)
		{
		}
	};

	typedef std::list<TreeNode*> TreeNodeCollection;

#define IS_CHILD_OF(Pointer, Type) (dynamic_cast<Type*>(Pointer) != nullptr)
}

#endif