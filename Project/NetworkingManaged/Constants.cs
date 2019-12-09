// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;

namespace GameFramework.Networking
{
	static class Constants
	{
		public static class Control
		{
			public const int SIZE = sizeof(byte);

			public const byte BUFFER = 1;
			public const byte HANDSHAKE = 2;
			public const byte HANDSHAKE_BACK = 2;
			public const byte PING = 3;
		}

		public static class UDP
		{
			public const uint PACKET_HEADER_SIZE = sizeof(ulong) + sizeof(bool) + sizeof(ulong) + sizeof(ushort) + sizeof(ushort);
		}

		public const uint RECEIVE_TIMEOUT = 1;
		public const uint SEND_TIMEOUT = 1;
		public const uint RECEIVE_BUFFER_SIZE = 8 * 1024;
		public const uint SEND_BUFFER_SIZE = 8 * 1024;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		public const ushort TIME_TO_LIVE = 64;
		public const float PING_TIME = 5;

		public const uint UDP_MAX_MTU = 1500;

		public const uint DEFAULT_PACKET_RATE = 1024;

		public static readonly Random Random = new Random();
	}
}