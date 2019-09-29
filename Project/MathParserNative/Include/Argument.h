// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef ARGUMENT_H
#define ARGUMENT_H

#include <string>
#include <vector>

namespace Zorvan::Framework::MathParser
{
	class Argument
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