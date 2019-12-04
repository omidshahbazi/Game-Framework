// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using GameFramework.Common.Utilities;

namespace GameFramework.Networking
{
	static class Constants
	{
		public static class Control
		{
			public const int SIZE = sizeof(byte);

			public const byte BUFFER = 1;
			public const byte CONNECTION = 2;
			public const byte PING = 3;
		}

		public static class Packet
		{
			public const uint PACKET_SIZE_SIZE = sizeof(uint);
			public const uint HEADER_SIZE = Control.SIZE;

			public static BufferStream CreateOutgoingBufferStream(uint Length)
			{
				uint length = HEADER_SIZE + Length;

				BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
				buffer.ResetWrite();
				buffer.WriteUInt32(length);
				buffer.WriteBytes(Control.BUFFER);

				return buffer;
			}

			public static BufferStream CreateIncommingBufferStream(byte[] Buffer)
			{
				return new BufferStream(Buffer, HEADER_SIZE, (uint)(Buffer.Length - HEADER_SIZE));
			}

			public static BufferStream CreateConnectionBufferStream()
			{
				uint length = HEADER_SIZE;

				BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
				buffer.ResetWrite();
				buffer.WriteUInt32(length);
				buffer.WriteBytes(Control.CONNECTION);

				return buffer;
			}

			public static BufferStream CreatePingBufferStream()
			{
				uint length = HEADER_SIZE + sizeof(double);

				BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
				buffer.ResetWrite();
				buffer.WriteUInt32(length);
				buffer.WriteBytes(Control.PING);
				buffer.WriteFloat64(Time.CurrentEpochTime);

				return buffer;
			}
		}

		public const uint RECEIVE_TIMEOUT = 1;
		public const uint SEND_TIMEOUT = 1;
		public const uint RECEIVE_BUFFER_SIZE = 8 * 1024;
		public const uint SEND_BUFFER_SIZE = 8 * 1024;

		// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.ttl?view=netframework-4.8#System_Net_Sockets_Socket_Ttl
		public const ushort TIME_TO_LIVE = 64;
		public const float PING_TIME = 5;

		public static readonly Random Random = new Random();
	}
}