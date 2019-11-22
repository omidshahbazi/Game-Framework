// Copyright 2019. All Rights Reserved.
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	const int Constants::Control::SIZE = sizeof(byte);

	const byte Constants::Control::BUFFER = (byte)1;
	const byte Constants::Control::PING = (byte)2;

	const uint32_t Constants::Packet::PACKET_SIZE_SIZE = sizeof(uint32_t);
	const uint32_t Constants::Packet::HEADER_SIZE = Control::SIZE;

	BufferStream Constants::Packet::CreateOutgoingBufferStream(uint32_t Length)
	{
		uint32_t length = HEADER_SIZE + Length;

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.Reset();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Control::BUFFER);

		return buffer;
	}

	BufferStream Constants::Packet::CreateIncommingBufferStream(byte* const Buffer, uint32_t Length)
	{
		return BufferStream(Buffer, HEADER_SIZE, (uint32_t)(Length - HEADER_SIZE));
	}

	BufferStream Constants::Packet::CreatePingBufferStream(void)
	{
		uint32_t length = HEADER_SIZE + sizeof(double);

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.Reset();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Control::PING);
		buffer.WriteFloat64(Time::GetCurrentEpochTime());

		return buffer;
	}

	const uint32_t Constants::RECEIVE_TIMEOUT = 1;
	const uint32_t Constants::SEND_TIMEOUT = 1;
	constexpr uint32_t Constants::RECEIVE_BUFFER_SIZE = 8 * 1024;
	const uint32_t Constants::SEND_BUFFER_SIZE = 8 * 1024;
	const uint16_t Constants::TIME_TO_LIVE = 64;
	const float Constants::PING_TIME = 5;
}