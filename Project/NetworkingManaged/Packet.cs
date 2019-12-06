// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;

namespace GameFramework.Networking
{
	public static class Packet
	{
		public const uint PACKET_SIZE_SIZE = sizeof(uint);
		public const uint HEADER_SIZE = Constants.Control.SIZE;

		public static BufferStream CreateOutgoingBufferStream(uint Length)
		{
			uint length = HEADER_SIZE + Length;

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.BUFFER);

			return buffer;
		}

		public static BufferStream CreateIncommingBufferStream(byte[] Buffer)
		{
			return new BufferStream(Buffer, HEADER_SIZE, (uint)(Buffer.Length - HEADER_SIZE));
		}

		public static BufferStream CreateHandshakeBufferStream(uint MTU)
		{
			uint length = HEADER_SIZE + sizeof(uint);

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.HANDSHAKE);
			buffer.WriteUInt32(MTU);

			return buffer;
		}

		public static BufferStream CreateHandshakeBackBufferStream(uint PacketRate)
		{
			uint length = HEADER_SIZE + sizeof(uint);

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.HANDSHAKE_BACK);
			buffer.WriteUInt32(PacketRate);

			return buffer;
		}

		public static BufferStream CreatePingBufferStream()
		{
			uint length = HEADER_SIZE + sizeof(double);

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.PING);
			buffer.WriteFloat64(Time.CurrentEpochTime);

			return buffer;
		}
	}

	public class RUDPPacket
	{
		public ulong ID
		{
			get;
			private set;
		}

		public BufferStream[] Buffers
		{
			get;
			private set;
		}

		public bool IsCompleted
		{
			get
			{
				for (uint i = 0; i < Buffers.Length; ++i)
					if (Buffers[i] == null)
						return false;

				return true;
			}
		}

		public RUDPPacket(ulong ID, uint Count)
		{
			this.ID = ID;

			Buffers = new BufferStream[Count];
		}

		public void SetBuffer(uint Index, BufferStream Buffer)
		{
			if (Buffers[Index] != null)
				return;

			Buffers[Index] = Buffer;
		}

		public BufferStream Combine()
		{
			BufferStream buffer = Buffers[0];

			for (uint i = 1; i < Buffers.Length; ++i)
				buffer.WriteBytes(Buffers[i].Buffer);

			return buffer;
		}
	}
}