// Copyright 2019. All Rights Reserved.
#pragma once

#include <vector>
#include <atomic>

#ifdef NETWORKING_API
#undef NETWORKING_API
#endif

#ifdef BUILD_DLL
#define NETWORKING_API __declspec(dllexport)
#else
#define NETWORKING_API
#endif

template<typename T>
using List = std::vector<T>;

template<typename T>
struct WaitFor
{
	WaitFor(std::atomic<T>& Value) :
		m_Value(&Value)
	{
		bool expected = false;
		while (m_Value->compare_exchange_weak(expected, true));

		m_Value->exchange(true);
	}

	~WaitFor(void)
	{
		m_Value->exchange(false);
	}

private:
	std::atomic<T>* m_Value;
};

#define WAIT_FOR_BOOL(AtomicValue) WaitFor wait_##AtomicValue(AtomicValue)