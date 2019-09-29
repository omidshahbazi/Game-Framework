// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#include "..\Include\PredefinedFunctions.h"
#include <sstream>

namespace Zorvan::Framework::MathParser
{
	bool PredefinedFunctions::m_Initialized = false;
	PredefinedFunctions::FunctionsMap PredefinedFunctions::m_Functions;

	double PredefinedFunctions::Calculate(std::string Name, const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		if (m_Functions.find(Name) == m_Functions.end())
		{
			std::stringstream ss;
			ss << "Function [" << Name << "] not found";
			throw std::exception(ss.str().c_str());
		}

		return m_Functions[Name].Calculate(Arguments, ArgsMap);
	}

	void PredefinedFunctions::CheckArguments(TreeNodeCollection Arguments, const std::string &Name, int DesiredCount)
	{
		if (Arguments.size() == DesiredCount)
			return;

		std::stringstream ss;
		ss << "Arguments counts in [" << Name << "] should be [" << DesiredCount << "], but there are [" << Arguments.size() << "]";

		throw std::exception(ss.str().c_str());
	}

	double PredefinedFunctions::Function_if(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "if", 3);

		auto it = Arguments.begin();

		double condition = TreeNodeCalculator::Calculate(*it, ArgsMap);

		++it;
		if (condition == 1)
			return TreeNodeCalculator::Calculate(*it, ArgsMap);

		++it;
		return TreeNodeCalculator::Calculate(*it, ArgsMap);
	}

	double PredefinedFunctions::Function_log(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap & ArgsMap)
	{
		CheckArguments(Arguments, "log", 1);

		return log10f(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_abs(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap & ArgsMap)
	{
		CheckArguments(Arguments, "abs", 1);

		return fabs(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_acos(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "acos", 1);

		return acosf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_asin(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "asin", 1);

		return asinf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_atan(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "atan", 1);

		return atanf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_cos(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "cos", 1);

		return cosf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_cosh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "cosh", 1);

		return coshf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_sin(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "sin", 1);

		return sinf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_sinh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "sinh", 1);

		return sinhf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_tan(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "tan", 1);

		return tanf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_tanh(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "tanh", 1);

		return tanh(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_ceiling(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "ceiling", 1);

		return ceilf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_exp(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "exp", 1);

		return expf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_floor(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "floor", 1);

		return floorf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_Round(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "round", 1);

		return roundf(TreeNodeCalculator::Calculate(*Arguments.begin(), ArgsMap));
	}

	double PredefinedFunctions::Function_max(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "max", 2);

		auto it = Arguments.begin();
		double leftValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		++it;
		double rightValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		return (leftValue > rightValue ? leftValue : rightValue);
	}

	double PredefinedFunctions::Function_min(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "min", 2);

		auto it = Arguments.begin();
		double leftValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		++it;
		double rightValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		return (leftValue < rightValue ? leftValue : rightValue);
	}

	double PredefinedFunctions::Function_random(const TreeNodeCollection &Arguments, const TreeNodeCalculator::ArgumentsMap &ArgsMap)
	{
		CheckArguments(Arguments, "random", 2);

		auto it = Arguments.begin();
		double leftValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		++it;
		double rightValue = TreeNodeCalculator::Calculate(*it, ArgsMap);

		if (leftValue == rightValue)
			return leftValue;

		return fmodf(rand(), rightValue - leftValue) + leftValue;
	}
}