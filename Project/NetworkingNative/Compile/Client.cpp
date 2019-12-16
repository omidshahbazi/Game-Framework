// Copyright 2019. All Rights Reserved.
#pragma once

#include "..\Include\Client.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	bool Client::GetIsReady(void) const
	{
		return (Time::GetCurrentEpochTime() - GetStatistics().GetLastTouchTime() < Constants::PING_TIME * 2);
	}
}