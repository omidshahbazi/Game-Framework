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
		public const ushort PACKET_SIZE_SIZE = sizeof(uint);
		public const ushort HEADER_SIZE = Constants.Control.SIZE;

		public static BufferStream CreateOutgoingBufferStream(uint Length)
		{
			uint length = HEADER_SIZE + Length;

			BufferStream buffer = new BufferStream(new byte[PACKET_SIZE_SIZE + length]);
			buffer.ResetWrite();
			buffer.WriteUInt32(length);
			buffer.WriteBytes(Constants.Control.BUFFER);

			return buffer;
		}

		public static BufferStream CreateIncomingBufferStream(byte[] Buffer)
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
		public class PacketMap : SortedDictionary<ulong, T>
		{ }

		public PacketMap PacketsMap
		{
			get;
			private set;
		}

		public ulong LastID
		{
			get;
			protected set;
		}

		public UDPPacketsHolder()
		{
			PacketsMap = new PacketMap();
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

		public static uint GetAckMask(UDPPacketsHolder<T> IncomingHolder, uint AckMask)
		{
			ushort bitCount = Constants.UDP.LAST_ACK_MASK_SIZE * 8;

			for (ushort i = 0; i < bitCount; ++i)
			{
				ushort offset = (ushort)(i + 1);

				if (offset > IncomingHolder.LastID)
					break;

				ulong packetID = IncomingHolder.LastID - offset;

				T packet = null;
				IncomingHolder.PacketsMap.TryGetValue(packetID, out packet);

				if (packet == null || packet.IsCompleted)
					AckMask = BitwiseHelper.Enable(AckMask, i);
			}

			return AckMask;
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

		public static OutgoingUDPPacket Create(OutgoingUDPPacketsHolder OutgoingHolder, IncomingUDPPacketsHolder IncomingHolder, byte[] Buffer, uint Index, uint Length, uint MTU, bool IsReliable)
		{
			if (Constants.UDP.PACKET_HEADER_SIZE >= MTU)
				throw new Exception("PACKET_HEADER_SIZE [" + Constants.UDP.PACKET_HEADER_SIZE + "] is greater than or equal to MTU [" + MTU + "]");

			OutgoingHolder.IncreaseLastID();

			ulong id = OutgoingHolder.LastID;

			uint mtu = MTU - Constants.UDP.PACKET_HEADER_SIZE;

			ushort sliceCount = (ushort)Math.Ceiling(Length / (float)mtu);

			OutgoingUDPPacket packet = new OutgoingUDPPacket(id, sliceCount, IsReliable);

			uint ackMask = OutgoingHolder.AckMask;
			ackMask = ackMask << 1;
			ackMask = UDPPacketsHolder<IncomingUDPPacket>.GetAckMask(IncomingHolder, ackMask);

			for (ushort i = 0; i < sliceCount; ++i)
			{
				uint index = Index + (i * mtu);
				uint length = (uint)Math.Min(mtu, Length - (i * mtu));

				BufferStream buffer = Packet.CreateOutgoingBufferStream(Constants.UDP.PACKET_HEADER_SIZE + length);
				buffer.WriteUInt64(IncomingHolder.LastID);
				buffer.WriteUInt32(ackMask);
				buffer.WriteBool(IsReliable);
				buffer.WriteUInt64(id);
				buffer.WriteUInt16(sliceCount);
				buffer.WriteUInt16(i);
				buffer.WriteBytes(Buffer, index, length);

				packet.SetSliceBuffer(i, buffer);
			}

			OutgoingHolder.AddPacket(packet);

			return packet;
		}
	}

	class IncomingUDPPacketsHolder : UDPPacketsHolder<IncomingUDPPacket>
	{
		public ulong PrevID
		{
			get;
			private set;
		}

		public void SetLastID(ulong Value)
		{
			LastID = Value;
		}

		public void SetPrevID(ulong Value)
		{
			PrevID = Value;
		}

		public static void ProcessReliablePackets(IncomingUDPPacketsHolder Holder, Action<BufferStream> HandleReceivedBuffer)
		{
			List<ulong> completedIDs = new List<ulong>();

			var it = Holder.PacketsMap.GetEnumerator();
			while (it.MoveNext())
			{
				ulong id = it.Current.Key;
				IncomingUDPPacket packet = it.Current.Value;

				if (!it.Current.Value.IsCompleted)
					break;

				if (id < Holder.PrevID)
				{
					completedIDs.Add(id);
					continue;
				}

				if (id - Holder.PrevID > 1)
					break;

				HandleReceivedBuffer(packet.Combine());

				Holder.SetPrevID(id);

				completedIDs.Add(id);

			}

			for (int i = 0; i < completedIDs.Count; ++i)
				Holder.PacketsMap.Remove(completedIDs[i]);
		}

		public static void ProcessNonReliablePacket(IncomingUDPPacketsHolder Holder, IncomingUDPPacket Packet, Action<BufferStream> HandleReceivedBuffer)
		{
			HandleReceivedBuffer(Packet.Combine());

			Holder.PacketsMap.Remove(Packet.ID);
		}
	}

	class OutgoingUDPPacketsHolder : UDPPacketsHolder<OutgoingUDPPacket>
	{
		public ulong LastAckID
		{
			get;
			private set;
		}

		public uint AckMask
		{
			get;
			private set;
		}

		public void IncreaseLastID()
		{
			++LastID;
		}

		public void SetLastAckID(ulong Value)
		{
			LastAckID = Value;
		}

		public void SetAckMask(uint Value)
		{
			AckMask = Value;
		}

		//public static void ProcessReliablePackets(OutgoingUDPPacketsHolder Holder, Action<OutgoingUDPPacket> SendPacket)
		//{
		//	ulong lastAckID = Holder.LastAckID;

		//	if (lastAckID == 0)
		//		return;

		//	short count = (short)Math.Min(lastAckID - 1, Constants.UDP.LAST_ACK_MASK_SIZE * 8);

		//	for (short i = count; i >= 0; --i)
		//	{
		//		ulong id = (ulong)((long)lastAckID - i);

		//		bool acked = (BitwiseHelper.IsEnabled(Holder.AckMask, (ushort)i) || id == lastAckID);

		//		if (acked)
		//		{
		//			Holder.PacketsMap.Remove(id);
		//			continue;
		//		}

		//		OutgoingUDPPacket packet = Holder.PacketsMap[id];

		//		SendPacket(packet);
		//	}
		//}

		public static void ProcessReliablePackets(OutgoingUDPPacketsHolder Holder, Action<OutgoingUDPPacket> SendPacket)
		{
			ulong lastAckID = Holder.LastAckID;

			if (lastAckID == 0)
				return;

			short count = (short)Math.Min(Holder.LastID - 1, Constants.UDP.LAST_ACK_MASK_SIZE * 8);

			for (short i = count; i >= 0; --i)
			{
				ulong id = (ulong)((long)lastAckID - i);

				bool acked = (BitwiseHelper.IsEnabled(Holder.AckMask, (ushort)i) || id == lastAckID);

				if (acked)
				{
					Holder.PacketsMap.Remove(id);
					continue;
				}

				OutgoingUDPPacket packet = Holder.PacketsMap[id];

				SendPacket(packet);
			}
		}

		public static void ProcessNonReliablePackets(OutgoingUDPPacketsHolder Holder, Action<OutgoingUDPPacket> SendPacket)
		{
			ProcessReliablePackets(Holder, SendPacket);
		}
	}
}