// Copyright 2019. All Rights Reserved.
#include "..\Include\Constants.h"

namespace GameFramework::Networking
{
	const int Constants::Control::SIZE = sizeof(byte);

	const byte Constants::Control::BUFFER = (byte)1;
	const byte Constants::Control::PING = (byte)2;

	const uint32_t Constants::Packet::PACKET_SIZE_SIZE = sizeof(uint32_t32_t);
	const uint32_t Constants::Packet::HEADER_SIZE = Control::SIZE;

	BufferStream Constants::Packet::CreateOutgoingBufferStream(uint32_t Length)
	{
		uint32_t length = HEADER_SIZE + Length;

		BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
		buffer.Reset();
		buffer.Writeuint32_t32(length);
		buffer.WriteBytes(Control::BUFFER);

		return buffer;
	}

	BufferStream Constants::Packet::CreateIncommingBufferStream(byte[] Buffer)
	{
		return new BufferStream(Buffer, HEADER_SIZE, (uint32_t)(Buffer.Length - HEADER_SIZE));
	}

	BufferStream Constants::Packet::CreatePingBufferStream(void)
	{
		uint32_t length = HEADER_SIZE + sizeof(double);

		BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
		buffer.Reset();
		buffer.Writeuint32_t32(length);
		buffer.WriteBytes(Control::PING);
		buffer.WriteFloat64(Time.CurrentEpochTime);

		return buffer;
	}

	const uint32_t Constants::RECEIVE_TIMEOUT = 1;
	const uint32_t Constants::SEND_TIMEOUT = 1;
	const uint32_t Constants::RECEIVE_BUFFER_SIZE = 8 * 1024;
	const uint32_t Constants::SEND_BUFFER_SIZE = 8 * 1024;
	const short Constants::TIME_TO_LIVE = 64;
	const float Constants::PING_TIME = 5;

	//static Random Random = new Random();
}