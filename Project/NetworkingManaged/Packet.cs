// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System;
using System.Collections.Generic;

namespace GameFramework.Networking
{
	static class Packet
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

	abstract class RUDPPacket
	{
		public ulong ID
		{
			get;
			private set;
		}

		public BufferStream[] SliceBuffers
		{
			get;
			private set;
		}

		public uint Length
		{
			get
			{
				uint length = 0;

				for (uint i = 0; i < SliceBuffers.Length; ++i)
				{
					BufferStream buffer = SliceBuffers[i];

					if (buffer == null)
						continue;

					length += buffer.Size;
				}

				return length;
			}
		}

		public bool IsReliable
		{
			get;
			private set;
		}

		public abstract bool IsCompleted
		{
			get;
		}

		public RUDPPacket(ulong ID, uint SliceCount, bool IsReliable)
		{
			this.ID = ID;

			SliceBuffers = new BufferStream[SliceCount];

			this.IsReliable = IsReliable;
		}

		public void SetSliceBuffer(uint Index, BufferStream Buffer)
		{
			if (SliceBuffers[Index] != null)
				return;

			SliceBuffers[Index] = Buffer;
		}
	}

	class IncommingRUDPPacket : RUDPPacket
	{
		public override bool IsCompleted
		{
			get
			{
				for (uint i = 0; i < SliceBuffers.Length; ++i)
					if (SliceBuffers[i] == null)
						return false;

				return true;
			}
		}

		public IncommingRUDPPacket(ulong ID, uint SliceCount, bool IsReliable) : base(ID, SliceCount, IsReliable)
		{
		}

		public BufferStream Combine()
		{
			BufferStream buffer = new BufferStream(Length);

			for (uint i = 0; i < SliceBuffers.Length; ++i)
				buffer.WriteBytes(SliceBuffers[i].Buffer);

			return buffer;
		}
	}

	class IncommingPacketMap : SortedDictionary<ulong, IncommingRUDPPacket>
	{ }

	class OutgoingRUDPPacket : RUDPPacket
	{
		public override bool IsCompleted
		{
			get { return false; }
		}

		public OutgoingRUDPPacket(ulong ID, ushort SliceCount, bool IsReliable) : base(ID, SliceCount, IsReliable)
		{
		}

		public void SetSliceBuffer(ushort Index, BufferStream Buffer)
		{
			if (SliceBuffers[Index] != null)
				return;

			SliceBuffers[Index] = Buffer;
		}

		public static OutgoingRUDPPacket Create(ulong ID, byte[] Buffer, uint Index, uint Length, uint MTU, bool IsReliable)
		{
			if (Constants.UDP.PACKET_HEADER_SIZE >= MTU)
				throw new Exception("PACKET_HEADER_SIZE [" + Constants.UDP.PACKET_HEADER_SIZE + "] is greater than or equal to MTU [" + MTU + "]");

			uint mtu = MTU - Constants.UDP.PACKET_HEADER_SIZE;

			ushort sliceCount = (ushort)Math.Ceiling(Length / (float)mtu);

			OutgoingRUDPPacket pakcet = new OutgoingRUDPPacket(ID, sliceCount, IsReliable);

			for (ushort i = 0; i < sliceCount; ++i)
			{
				uint index = Index + (i * mtu);
				uint length = (uint)Math.Min(mtu, Length - (i * mtu));

				BufferStream buffer = Packet.CreateOutgoingBufferStream(Constants.UDP.PACKET_HEADER_SIZE + length);
				buffer.WriteBool(IsReliable);
				buffer.WriteUInt64(ID);
				buffer.WriteUInt16(sliceCount);
				buffer.WriteUInt16(i);
				buffer.WriteBytes(Buffer, index, length);

				pakcet.SetSliceBuffer(i, buffer);
			}

			return pakcet;
		}
	}
}