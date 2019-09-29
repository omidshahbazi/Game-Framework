// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef FUNCTIONDEFINITION_H
#define FUNCTIONDEFINITION_H

#include "SyntaxTree\TreeNode.h"
#include "TreeNodeCalculator.h"
#include <functional>

using namespace Zorvan::Framework::MathParser;
using namespace Zorvan::Framework::MathParser::SyntaxTree;

namespace Zorvan::Framework::MathParser
{
	class FunctionDefinition
	{
	public:
		typedef std::function<double(const TreeNodeCollection&, const TreeNodeCalculator::ArgumentsMap&)> FunctionSignature;

	public:
		FunctionDefinition(void)
		{
		}

		FunctionDefinition(const char *Name, const char *Description, FunctionSignature Function) :
			m_Name(Name),
			m_Description(Description),
			m_Function(Function)
		{
		}

		double Calculate(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap) const
		{
			return m_Function(Arguments, ArgsMap);
		}

		const char *GetName(void) const
		{
			return m_Name;
		}

		const char *GetDescription(void) const
		{
			return m_Description;
		}

	private:
		const char *m_Name;
		const char *m_Description;
		FunctionSignature m_Function;
	};
}

#endif