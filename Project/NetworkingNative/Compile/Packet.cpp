// Copyright 2019. All Rights Reserved.
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	const uint16_t Packet::PACKET_SIZE_SIZE = sizeof(uint32_t);
	const uint16_t Packet::HEADER_SIZE = Constants::Control::SIZE;

	const uint16_t Packet::PING_SIZE = Packet::PACKET_SIZE_SIZE + Packet::HEADER_SIZE + sizeof(double);

	BufferStream Packet::CreateOutgoingBufferStream(uint32_t Length)
	{
		uint32_t length = HEADER_SIZE + Length;

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::BUFFER);

		return buffer;
	}

	BufferStream Packet::CreateIncomingBufferStream(std::byte* const Buffer, uint32_t Length)
	{
		return BufferStream(Buffer, HEADER_SIZE, (uint32_t)(Length - HEADER_SIZE));
	}

	BufferStream Packet::CreatePingBufferStream(uint32_t PayloadSize)
	{
		uint32_t length = (PING_SIZE - PACKET_SIZE_SIZE) + PayloadSize;

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::PING);
		buffer.WriteFloat64(Time::GetCurrentEpochTime());

		return buffer;
	}
}