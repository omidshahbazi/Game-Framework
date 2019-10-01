// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef EXPRESSION_H
#define EXPRESSION_H

#include "Argument.h"
#include "TreeNodeCalculator.h"
#include <string>
#include <map>
#include <sstream>

namespace GameFramework::MathParser
{
	namespace SyntaxTree
	{
		class TreeNode;
	}

	class Expression
	{
	private:

	public:
		Expression(const std::string &Expr);

		double Calculate(void)
		{
			return Calculate({});
		}

		double Calculate(const ArgumentCollection &Arguments);

		std::string ToString(void) const;

	private:
		static void Print(TreeNode *Node, int Indent, std::stringstream &Builder);
		static void PrintLine(const std::string &Text, int Indent, std::stringstream &Builder);

	private:
		SyntaxTree::TreeNode *m_Node;
	};
}

#endif