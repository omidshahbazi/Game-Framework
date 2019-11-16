// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TIME_H
#define TIME_H

#include "..\Common.h"

namespace GameFramework::Common::Timing
{
	static class COMMON_API Time
	{
	public:
		static double GetCurrentEpochTime(void);
	};
}

#endif