// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using GameFramework.Common.Utilities;
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

		public static BufferStream CreateHandshakeBackBufferStream(uint PacketCountRate)
		{
			uint length = HEADER_SIZE + sizeof(uint);

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.HANDSHAKE_BACK);
			buffer.WriteUInt32(PacketCountRate);

			return buffer;
		}

		public static BufferStream CreatePingBufferStream(uint PayloadSize = 0)
		{
			uint length = HEADER_SIZE + sizeof(double);

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length + PayloadSize]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.PING);
			buffer.WriteFloat64(Time.CurrentEpochTime);

			return buffer;
		}
	}

	abstract class UDPPacket
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

		public UDPPacket(ulong ID, uint SliceCount, bool IsReliable)
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

	class UDPPacketsHolder<T> where T : UDPPacket
	{
		public class PacketMap : Dictionary<ulong, T>
		{ }

		public PacketMap PacketsMap
		{
			get;
			private set;
		}

		public ulong LastID
		{
			get;
			private set;
		}

		public ulong AckMask
		{
			get;
			private set;
		}

		public UDPPacketsHolder()
		{
			PacketsMap = new PacketMap();
			LastID = 1;
		}

		public T GetPacket(ulong ID)
		{
			if (PacketsMap.ContainsKey(ID))
				return PacketsMap[ID];

			return null;
		}

		public void AddPacket(T Packet)
		{
			PacketsMap[Packet.ID] = Packet;
		}

		public void SetLastID(ulong Value)
		{
			LastID = Value;
		}

		public void SetAckMask(uint Value)
		{
			AckMask = Value;
		}

		public void IncreaseLastID()
		{
			++LastID;
		}

		public static uint GetAckMask(UDPPacketsHolder<T> IncomingHolder)
		{
			uint mask = 0;

			ushort bitCount = sizeof(uint) * 8;

			for (ushort i = 0; i < bitCount; ++i)
			{
				ushort offset = (ushort)(i + 1);

				if (offset > IncomingHolder.LastID)
					break;

				ulong packetID = IncomingHolder.LastID - offset;

				T packet = null;
				IncomingHolder.PacketsMap.TryGetValue(packetID, out packet);

				if (packet != null && packet.IsCompleted)
					mask = BitwiseHelper.Enable(mask, i);
			}

			return mask;
		}
	}

	class IncomingUDPPacket : UDPPacket
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

		public IncomingUDPPacket(ulong ID, uint SliceCount, bool IsReliable) : base(ID, SliceCount, IsReliable)
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

	class OutgoingUDPPacket : UDPPacket
	{
		public override bool IsCompleted
		{
			get { return false; }
		}

		public OutgoingUDPPacket(ulong ID, ushort SliceCount, bool IsReliable) : base(ID, SliceCount, IsReliable)
		{
		}

		public void SetSliceBuffer(ushort Index, BufferStream Buffer)
		{
			if (SliceBuffers[Index] != null)
				return;

			SliceBuffers[Index] = Buffer;
		}

		public static OutgoingUDPPacket Create(ulong ID, IncomingUDPPacketsHolder IncomingHolder, byte[] Buffer, uint Index, uint Length, uint MTU, bool IsReliable)
		{
			if (Constants.UDP.PACKET_HEADER_SIZE >= MTU)
				throw new Exception("PACKET_HEADER_SIZE [" + Constants.UDP.PACKET_HEADER_SIZE + "] is greater than or equal to MTU [" + MTU + "]");

			uint mtu = MTU - Constants.UDP.PACKET_HEADER_SIZE;

			ushort sliceCount = (ushort)Math.Ceiling(Length / (float)mtu);

			OutgoingUDPPacket pakcet = new OutgoingUDPPacket(ID, sliceCount, IsReliable);

			uint ackMask = UDPPacketsHolder<IncomingUDPPacket>.GetAckMask(IncomingHolder);

			for (ushort i = 0; i < sliceCount; ++i)
			{
				uint index = Index + (i * mtu);
				uint length = (uint)Math.Min(mtu, Length - (i * mtu));

				BufferStream buffer = Packet.CreateOutgoingBufferStream(Constants.UDP.PACKET_HEADER_SIZE + length);
				buffer.WriteUInt64(IncomingHolder.LastID);
				buffer.WriteUInt32(ackMask);
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

	class IncomingUDPPacketsHolder : UDPPacketsHolder<IncomingUDPPacket>
	{ }

	class OutgoingUDPPacketsHolder : UDPPacketsHolder<OutgoingUDPPacket>
	{ }
}