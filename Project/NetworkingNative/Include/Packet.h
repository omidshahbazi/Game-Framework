// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef PACKET_H
#define PACKET_H

#include "Common.h"
#include <cstddef>
#include <stdint.h>
#include <BufferStream.h>

using namespace GameFramework::BinarySerializer;

namespace GameFramework::Networking
{
	static class NETWORKING_API Packet
	{
	public:
		static const uint16_t PACKET_SIZE_SIZE;
		static const uint16_t HEADER_SIZE;

		static const uint16_t PING_SIZE;

	public:
		static BufferStream CreateOutgoingBufferStream(uint32_t Length);

		static BufferStream CreateIncomingBufferStream(std::byte* const Buffer, uint32_t Length);

		static BufferStream CreateHandshakeBufferStream(uint32_t MTU);

		static BufferStream CreateHandshakeBackBufferStream(uint32_t PacketCountRate);

		static BufferStream CreatePingBufferStream(uint32_t PayloadSize = 0);
	};
}

#endif