// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CONSTANTS_H
#define CONSTANTS_H

#include "Common.h"
#include <cstddef>
#include <stdint.h>
#include <BufferStream.h>
#include <Utilities/Random.h>

using namespace std;
using namespace GameFramework::BinarySerializer;
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

			static const byte BUFFER;
			static const byte PING;
		};

	public:
		static class Packet
		{
		public:
			static const uint32_t PACKET_SIZE_SIZE;
			static const uint32_t HEADER_SIZE;

		public:
			static BufferStream CreateOutgoingBufferStream(uint32_t Length);

			static BufferStream CreateIncommingBufferStream(byte* const Buffer, uint32_t Length);

			static BufferStream CreatePingBufferStream(void);
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