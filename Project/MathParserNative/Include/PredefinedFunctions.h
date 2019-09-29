// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef PREFEFINEDFUNCTIONS_H
#define PREFEFINEDFUNCTIONS_H

#include "TreeNodeCalculator.h"
#include "FunctionDefinition.h"
#include <string>
#include <map>
#include <vector>

namespace Zorvan::Framework::MathParser
{
	class PredefinedFunctions
	{
	public:
		typedef std::map<std::string, FunctionDefinition> FunctionsMap;

	public:
		inline static void Initialize(void)
		{
			if (m_Initialized)
				return;

			m_Initialized = true;

			Register(FunctionDefinition("if", "if (condition, then expression, else expression)", Function_if));
			Register(FunctionDefinition("log", "log(x)", Function_log));
			Register(FunctionDefinition("abs", "abs(x)", Function_abs));
			Register(FunctionDefinition("acos", "acos(x)", Function_acos));
			Register(FunctionDefinition("asin", "asin(x)", Function_asin));
			Register(FunctionDefinition("atan", "atan(x)", Function_atan));
			Register(FunctionDefinition("cos", "cos(x)", Function_cos));
			Register(FunctionDefinition("cosh", "cosh(x)", Function_cosh));
			Register(FunctionDefinition("sin", "sin(x)", Function_sin));
			Register(FunctionDefinition("sinh", "sinh(x)", Function_sinh));
			Register(FunctionDefinition("tan", "tan(x)", Function_tan));
			Register(FunctionDefinition("tanh", "tanh(x)", Function_tanh));
			Register(FunctionDefinition("ceiling", "ceiling(x)", Function_ceiling));
			Register(FunctionDefinition("exp", "exp(x)", Function_exp));
			Register(FunctionDefinition("floor", "floor(x)", Function_floor));
			Register(FunctionDefinition("round", "round(x)", Function_Round));
			Register(FunctionDefinition("min", "min(x, y", Function_min));
			Register(FunctionDefinition("max", "max(x, y)", Function_max));
			Register(FunctionDefinition("random", "random(minimum, maximum)", Function_random));
		}

		static void Register(const FunctionDefinition &Function)
		{
			m_Functions[Function.GetName()] = Function;
		}

		static double Calculate(std::string Name, const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static void GetFunctions(std::vector<FunctionDefinition> Functions)
		{
			for each (const auto &func in m_Functions)
				Functions.push_back(func.second);
		}

	private:
		static void CheckArguments(TreeNodeCollection Arguments, const std::string &Name, int DesiredCount);

		static double Function_if(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_log(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap & ArgsMap);

		static double Function_abs(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap & ArgsMap);

		static double Function_acos(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_asin(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_atan(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_cos(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_cosh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_sin(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_sinh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_tan(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_tanh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_ceiling(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_exp(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_floor(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_Round(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_max(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_min(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

		static double Function_random(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap);

	private:
		static bool m_Initialized;
		static FunctionsMap m_Functions;
	};
}

#endif