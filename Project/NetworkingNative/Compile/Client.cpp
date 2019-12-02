// Copyright 2019. All Rights Reserved.
#pragma once

#include "..\Include\Client.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	Client::Client(Socket Socket, const IPEndPoint& EndPoint) :
		m_Socket(Socket),
		m_EndPoint(EndPoint),
		m_LastTouchTime(0),
		m_Latency(0)
	{
		m_LastTouchTime = Time::GetCurrentEpochTime();
	}

	bool Client::GetIsReady(void) const
	{
		return (SocketUtilities::GetIsReady(m_Socket) && (Time::GetCurrentEpochTime() - m_LastTouchTime < Constants::PING_TIME * 2));
	}
}