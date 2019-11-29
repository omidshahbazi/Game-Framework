// Copyright 2019. All Rights Reserved.
#pragma once

#include "..\Include\Client.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	Client::Client(Socket Socket) :
		m_Socket(Socket)
	{
		m_LastTouchTime = Time::GetCurrentEpochTime();
	}

	bool Client::IsReady(void) const
	{
		return SocketUtilities::IsReady(m_Socket); //&& (Time.CurrentEpochTime - LastTouchTime < Constants.PING_TIME * 2)
	}
}