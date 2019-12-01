// Copyright 2019. All Rights Reserved.
#include "..\..\Include\Timing\Time.h"
#include <chrono>
#include <time.h>

using namespace std;
using namespace std::chrono;

namespace GameFramework::Common::Timing
{
	double Time::GetCurrentEpochTime(void)
	{
		time_point now = system_clock::now();

		return (double)now.time_since_epoch().count() * system_clock::period::num / system_clock::period::den;
	}
}