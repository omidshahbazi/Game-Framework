// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef ARGUMENT_H
#define ARGUMENT_H

#include "Common.h"
#include <string>
#include <vector>

namespace GameFramework::MathParser
{
	class MATH_PARSER_API Argument
	{
	public:
		Argument(const std::string &Name, double Value) :
			m_Name(Name),
			m_Value(Value)
		{
		}

		const std::string &GetName(void) const
		{
			return m_Name;
		}

		const double &GetValue(void) const
		{
			return m_Value;
		}

		void SetName(const std::string &Name)
		{
			m_Name = Name;
		}

		void SetValue(const double &Value)
		{
			m_Value = Value;
		}

	private:
		std::string m_Name;
		double m_Value;
	};

	typedef std::vector<Argument*> ArgumentCollection;
}

#endif