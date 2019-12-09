// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CONSTANTS_H
#define CONSTANTS_H

#include "Common.h"
#include <cstddef>
#include <stdint.h>
#include <Utilities/Random.h>

using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	static class NETWORKING_API Constants
	{
	public:
		static class Control
		{
		public:
			static const int SIZE;

			static const std::byte BUFFER;
			static const std::byte PING;
		};

	public:
		static const uint32_t RECEIVE_TIMEOUT;
		static const uint32_t SEND_TIMEOUT;
		static const uint32_t RECEIVE_BUFFER_SIZE;
		static const uint32_t SEND_BUFFER_SIZE;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		static const uint16_t TIME_TO_LIVE;
		static const float PING_TIME;

		static GameFramework::Common::Utilities::Random Random;
	};
}

#endif