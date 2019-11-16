// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef RANDOM_H
#define RANDOM_H

#include "..\Common.h"
#include <stdint.h>

namespace GameFramework::Common::Utilities
{
	class COMMON_API Random
	{
	public:
		Random(void);

		Random(int32_t Seed);

		int32_t Next(int32_t Min, int32_t Max);

		double NextDouble(void);

		__forceinline int32_t GetSeed(void) const
		{
			return m_Seed;
		}

	private:
		int32_t m_Seed;
	};
}

#endif