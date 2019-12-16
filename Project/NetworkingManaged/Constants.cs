// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;

namespace GameFramework.Networking
{
	static class Constants
	{
		public static class Control
		{
			public const ushort SIZE = sizeof(byte);

			public const byte BUFFER = 1;
			public const byte HANDSHAKE = 2;
			public const byte HANDSHAKE_BACK = 2;
			public const byte PING = 3;
		}

		public static class UDP
		{
			public const ushort LAST_ACK_ID_SIZE = sizeof(ulong);
			public const ushort ACK_MASK_SIZE = sizeof(uint);
			public const ushort IS_RELIABLE_SIZE = sizeof(bool);
			public const ushort ID_SIZE = sizeof(ulong);
			public const ushort SLICE_COUNT_SIZE = sizeof(ushort);
			public const ushort SLICE_INDEX_SIZE = sizeof(ushort);
			public const ushort PACKET_HEADER_SIZE = LAST_ACK_ID_SIZE + ACK_MASK_SIZE + IS_RELIABLE_SIZE + ID_SIZE + SLICE_COUNT_SIZE + SLICE_INDEX_SIZE;
		}

		public const uint RECEIVE_TIMEOUT = 1;
		public const uint SEND_TIMEOUT = 1;
		public const uint RECEIVE_BUFFER_SIZE = 8 * 1024;
		public const uint SEND_BUFFER_SIZE = 8 * 1024;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		public const ushort TIME_TO_LIVE = 64;
		public const float PING_TIME = 5;

		public const uint UDP_MAX_MTU = 1500;

		public const uint DEFAULT_PACKET_COUNT_RATE = 32;

		public static readonly Random Random = new Random();
	}
}