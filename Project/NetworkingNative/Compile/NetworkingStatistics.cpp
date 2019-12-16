// Copyright 2019. All Rights Reserved.
#pragma once

#include "..\Include\NetworkingStatistics.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	NetworkingStatistics::NetworkingStatistics(void) :
		m_LastTouchTime(Time::GetCurrentEpochTime()),
		m_Latency(0),
		m_ReceivedPacketFromLastSecond(0),
		m_BandwidthIn(0),
		m_BandwidthOut(0)
	{
	}
}