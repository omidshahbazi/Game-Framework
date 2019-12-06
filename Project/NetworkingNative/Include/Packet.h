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
		static const uint32_t PACKET_SIZE_SIZE;
		static const uint32_t HEADER_SIZE;

	public:
		static BufferStream CreateOutgoingBufferStream(uint32_t Length);

		static BufferStream CreateIncommingBufferStream(std::byte* const Buffer, uint32_t Length);

		static BufferStream CreatePingBufferStream(void);
	};
}

#endif