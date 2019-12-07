// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System.Net;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		private IncommingPacketMap incommingPacketsMap;
		private ulong lastOutgoingPacketID;

		public uint MTU
		{
			get;
			private set;
		}

		public uint PacketRate
		{
			get;
			private set;
		}

		public UDPClientSocket() : base(Protocols.UDP)
		{
			incommingPacketsMap = new IncommingPacketMap();
		}

		public virtual void Send(byte[] Buffer, bool Reliable = true)
		{
			Send(Buffer, 0, (uint)Buffer.Length, Reliable);
		}

		public virtual void Send(byte[] Buffer, uint Length, bool Reliable = true)
		{
			Send(Buffer, 0, Length, Reliable);
		}

		public virtual void Send(byte[] Buffer, uint Index, uint Length, bool Reliable = true)
		{
			OutgoingRUDPPacket packet = OutgoingRUDPPacket.Create(lastOutgoingPacketID, Buffer, Index, Length, MTU, Reliable);

			for (ushort i = 0; i < packet.SliceBuffers.Length; ++i)
				SendInternal(packet.SliceBuffers[i]);

			++lastOutgoingPacketID;
		}

		protected virtual void SendInternal(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer, Timestamp));
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.Connect(EndPoint);

			MTU = SocketUtilities.FindOptimumMTU(EndPoint.Address, Constants.UDP_MAX_MTU);
			//MTU = 17;  //TODO: it's just a hack to test

			BufferStream buffer = Packet.CreateHandshakeBufferStream(MTU);
			SendInternal(buffer);

			RunReceiveThread();
			RunSenndThread();

			//IsConnected = true;

			//RaiseOnConnectedEvent();
		}

		protected override void HandleIncommingBuffer(BufferStream Buffer)
		{
			LastTouchTime = Time.CurrentEpochTime;

			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Packet.CreateIncommingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(buffer);
			}
			else if (control == Constants.Control.HANDSHAKE_BACK)
			{
				PacketRate = Buffer.ReadUInt32();

				IsConnected = true;

				RaiseOnConnectedEvent();
			}
			else if (control == Constants.Control.PING)
			{
				HandlePingPacket(Buffer);
			}
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			bool isReliable = Buffer.ReadBool();
			ulong id = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			if (!isReliable && sliceCount == 1)
			{
				HandleReceivedBuffer(buffer);
				return;
			}

			IncommingRUDPPacket packet = GetIncommingPacket(id);
			if (packet == null)
			{
				packet = new IncommingRUDPPacket(id, sliceCount, isReliable);
				incommingPacketsMap[packet.ID] = packet;
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
				HandleReceivedBuffer(packet.Combine());
		}

		private IncommingRUDPPacket GetIncommingPacket(ulong ID)
		{
			if (incommingPacketsMap.ContainsKey(ID))
				return incommingPacketsMap[ID];

			return null;
		}
	}
}