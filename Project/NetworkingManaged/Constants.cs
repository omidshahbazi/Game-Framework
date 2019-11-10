// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;

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

			public static BufferStream CreatePingBufferStream()
			{
				return new BufferStream(new byte[Constants.Packet.HEADER_SIZE + sizeof(double)]);
			}

			public static void UpdatePingBufferStream(BufferStream Buffer)
			{
				Buffer.Reset();
				Buffer.WriteBytes(Control.PING);
				Buffer.WriteFloat64(Time.CurrentEpochTime);
			}
		}
	}
}