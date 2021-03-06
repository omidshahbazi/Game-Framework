// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TREENODE_H
#define TREENODE_H

#include "..\Common.h"
#include <list>

namespace GameFramework::MathParser::SyntaxTree
{
	class MATH_PARSER_API TreeNode
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