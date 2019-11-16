// Copyright 2019. All Rights Reserved.
#include "..\..\Include\Utilities\Random.h"
#include <random>

namespace GameFramework::Common::Utilities
{
	Random::Random(void) : Random(rand())
	{
	}

	Random::Random(int32_t Seed) :
		m_Seed(Seed)
	{
		srand(m_Seed);
	}

	int32_t Random::Next(int32_t Min, int32_t Max)
	{
		return Min + (rand() % (Max - Min));
	}

	double Random::NextDouble(void)
	{
		return Next(0, 100) / 100.0F;
	}
}