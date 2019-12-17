// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System.Collections.Generic;
using System.Net;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		private IncomingUDPPacketsHolder incomingReliablePacketHolder = null;
		private IncomingUDPPacketsHolder incomingNonReliablePacketHolder = null;

		private OutgoingUDPPacketsHolder outgoingReliablePacketHolder = null;
		private OutgoingUDPPacketsHolder outgoingNonReliablePacketHolder = null;

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
			incomingReliablePacketHolder = new IncomingUDPPacketsHolder();
			incomingNonReliablePacketHolder = new IncomingUDPPacketsHolder();
			outgoingReliablePacketHolder = new OutgoingUDPPacketsHolder();
			outgoingNonReliablePacketHolder = new OutgoingUDPPacketsHolder();
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
			OutgoingUDPPacketsHolder outgoingHolder = (Reliable ? outgoingReliablePacketHolder : outgoingNonReliablePacketHolder);
			IncomingUDPPacketsHolder incomingHolder = (Reliable ? incomingReliablePacketHolder : incomingNonReliablePacketHolder);

			OutgoingUDPPacket packet = OutgoingUDPPacket.CreateOutgoingBufferStream(outgoingHolder, incomingHolder, Buffer, Index, Length, MTU, Reliable);

			SendPacket(packet);
		}

		protected virtual void SendInternal(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer, Timestamp));
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.Connect(EndPoint);

			MTU = SocketUtilities.FindOptimumMTU(EndPoint.Address, 1000, Constants.UDP.MAX_MTU);

			BufferStream buffer = Packet.CreateHandshakeBufferStream(MTU);
			SendInternal(buffer);

			RunReceiveThread();
			RunSenndThread();
		}

		protected override void HandleIncomingBuffer(BufferStream Buffer)
		{
			Statistics.SetLatency((uint)Time.CurrentEpochTime);

			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Packet.CreateIncomingBufferStream(Buffer.Buffer);

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
			ulong lastAckID = Buffer.ReadUInt64();
			uint ackMask = Buffer.ReadUInt32();
			bool isReliable = Buffer.ReadBool();
			ulong packetID = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			IncomingUDPPacketsHolder incomingHolder = (isReliable ? incomingReliablePacketHolder : incomingNonReliablePacketHolder);

			IncomingUDPPacket packet = incomingHolder.GetPacket(packetID);
			if (packet == null)
			{
				packet = new IncomingUDPPacket(packetID, sliceCount, isReliable);
				incomingHolder.AddPacket(packet);
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
			{
				if (incomingHolder.LastID < packet.ID)
					incomingHolder.SetLastID(packet.ID);

				if (isReliable)
					ProcessIncomingReliablePackets();
				else
					ProcessIncomingNonReliablePacket(packet);
			}

			OutgoingUDPPacketsHolder outgoingHolder = (isReliable ? outgoingReliablePacketHolder : outgoingNonReliablePacketHolder);
			outgoingHolder.SetLastAckID(lastAckID);
			outgoingHolder.SetAckMask(ackMask);

			if (isReliable)
				ProcessOutgoingReliablePackets();
			else
				ProcessOutgoingNonReliablePackets();
		}

		private void SendPacket(OutgoingUDPPacket Packet)
		{
			for (ushort i = 0; i < Packet.SliceBuffers.Length; ++i)
				SendInternal(Packet.SliceBuffers[i]);
		}

		private void ProcessIncomingReliablePackets()
		{
			IncomingUDPPacketsHolder.ProcessReliablePackets(incomingReliablePacketHolder, HandleReceivedBuffer);
		}

		private void ProcessIncomingNonReliablePacket(IncomingUDPPacket Packet)
		{
			IncomingUDPPacketsHolder.ProcessNonReliablePacket(incomingNonReliablePacketHolder, Packet, HandleReceivedBuffer);
		}

		private void ProcessOutgoingReliablePackets()
		{
			OutgoingUDPPacketsHolder.ProcessReliablePackets(outgoingReliablePacketHolder, SendPacket);
		}

		private void ProcessOutgoingNonReliablePackets()
		{
			OutgoingUDPPacketsHolder.ProcessNonReliablePackets(outgoingNonReliablePacketHolder, SendPacket);
		}

		protected override void HandlePingPacketPayload(BufferStream Buffer)
		{
			base.HandlePingPacketPayload(Buffer);

			ulong lastAckID = Buffer.ReadUInt64();
			uint ackMask = Buffer.ReadUInt32();
			outgoingReliablePacketHolder.SetLastAckID(lastAckID);
			outgoingReliablePacketHolder.SetAckMask(ackMask);

			lastAckID = Buffer.ReadUInt64();
			ackMask = Buffer.ReadUInt32();
			outgoingNonReliablePacketHolder.SetLastAckID(lastAckID);
			outgoingNonReliablePacketHolder.SetAckMask(ackMask);
		}

		protected override BufferStream GetPingPacket()
		{
			return OutgoingUDPPacket.CreatePingBufferStream(outgoingReliablePacketHolder, incomingReliablePacketHolder, outgoingNonReliablePacketHolder, incomingNonReliablePacketHolder);
		}
	}
}