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
			static const uint16_t SIZE;

			static const std::byte BUFFER;
			static const std::byte HANDSHAKE;
			static const std::byte HANDSHAKE_BACK;
			static const std::byte PING;
		};

		static class UDP
		{
		public:
			static const uint32_t MAX_MTU;
			static const uint16_t LAST_ACK_ID_SIZE;
			static const uint16_t ACK_MASK_SIZE;
			static const uint16_t IS_RELIABLE_SIZE;
			static const uint16_t ID_SIZE;
			static const uint16_t SLICE_COUNT_SIZE;
			static const uint16_t SLICE_INDEX_SIZE;
			static const uint16_t PACKET_HEADER_SIZE;
		};

	public:
		static const uint32_t RECEIVE_TIMEOUT;
		static const uint32_t SEND_TIMEOUT;
		static const uint32_t RECEIVE_BUFFER_SIZE;
		static const uint32_t SEND_BUFFER_SIZE;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		static const uint16_t TIME_TO_LIVE;
		static const float PING_TIME;

		static const uint32_t DEFAULT_PACKET_COUNT_RATE;

		static GameFramework::Common::Utilities::Random Random;
	};
}

#endif