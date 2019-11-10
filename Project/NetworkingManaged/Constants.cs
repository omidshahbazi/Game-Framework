// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using GameFramework.Common.Utilities;

namespace GameFramework.NetworkingManaged
{
	static class Constants
	{
		public static class Control
		{
			public const int SIZE = sizeof(byte);

			public const byte BUFFER = 1;
			public const byte PING = 2;
		}

		public static class Packet
		{
			public const int HEADER_SIZE = Control.SIZE;

			public static BufferStream CreateOutgoingBufferStream(uint Length)
			{
				BufferStream buffer = new BufferStream(new byte[HEADER_SIZE + Length]);
				buffer.Reset();
				buffer.WriteBytes(Control.BUFFER);

				return buffer;
			}

			public static BufferStream CreateIncommingBufferStream(byte[] Buffer)
			{
				return new BufferStream(Buffer, HEADER_SIZE, (uint)(Buffer.Length - HEADER_SIZE));
			}

			public static BufferStream CreatePingBufferStream()
			{
				//BufferStream buffer = new BufferStream(new byte[HEADER_SIZE + sizeof(double)]);
				BufferStream buffer = new BufferStream(new byte[HEADER_SIZE + (sizeof(double) * 4)]);
				buffer.Reset();
				buffer.WriteBytes(Control.PING);
				buffer.WriteFloat64(Time.CurrentEpochTime);
				buffer.WriteFloat64(Time.CurrentEpochTime);
				buffer.WriteFloat64(Time.CurrentEpochTime);
				buffer.WriteFloat64(Time.CurrentEpochTime);

				return buffer;
			}
		}

		public const uint RECEIVE_TIMEOUT = 1;
		public const uint RECEIVE_BUFFER_SIZE = 1024;
		public const uint SEND_BUFFER_SIZE = 1024;
		public const float PING_TIME = 5;

		public static readonly Random Random = new Random();
	}
}