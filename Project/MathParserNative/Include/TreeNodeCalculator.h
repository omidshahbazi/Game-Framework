// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef TREENODECALCULATOR_H
#define TREENODECALCULATOR_H

#include "SyntaxTree\TreeNode.h"
#include <map>
#include <string>

using namespace Zorvan::Framework::MathParser::SyntaxTree;

namespace Zorvan::Framework::MathParser
{
	class TreeNodeCalculator
	{
	public:
		typedef std::map<std::string, double> ArgumentsMap;

	public:
		static double Calculate(TreeNode *Node, ArgumentsMap Arguments);
	};
}

#endif