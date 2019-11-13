﻿// Copyright 2019. All Rights Reserved.
#include <cstddef>
#include <stdint.h>

using namespace std;

namespace GameFramework::Networking
{
	static class Constants
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
			static const int PACKET_SIZE_SIZE;
			static const int HEADER_SIZE;

		public:
			static BufferStream CreateOutgoingBufferStream(uint32_t Length);

			static BufferStream CreateIncommingBufferStream(byte[] Buffer);

			static BufferStream CreatePingBufferStream(void);
		};

	public:
		static const uint32_t RECEIVE_TIMEOUT;
		static const uint32_t SEND_TIMEOUT;
		static const uint32_t RECEIVE_BUFFER_SIZE;
		static const uint32_t SEND_BUFFER_SIZE;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		static const short TIME_TO_LIVE;
		static const float PING_TIME;

		//static Random Random = new Random();
	};

	const float Constants::PING_TIME = 5;
}