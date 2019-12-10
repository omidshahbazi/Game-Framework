// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System.Net;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		private IncomingUDPPacketsHolder incomingReliablePacketHolder = null;
		private IncomingUDPPacketsHolder incomingPacketHolder = null;

		private OutgoingUDPPacketsHolder outgoingReliablePacketHolder = null;
		private OutgoingUDPPacketsHolder outgoingPacketHolder = null;

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
			incomingPacketHolder = new IncomingUDPPacketsHolder();
			outgoingReliablePacketHolder = new OutgoingUDPPacketsHolder();
			outgoingPacketHolder = new OutgoingUDPPacketsHolder();
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
			OutgoingUDPPacketsHolder outgoingHolder = (Reliable ? outgoingReliablePacketHolder : outgoingPacketHolder);
			IncomingUDPPacketsHolder incomingHolder = (Reliable ? incomingReliablePacketHolder : incomingPacketHolder);

			OutgoingUDPPacket packet = OutgoingUDPPacket.Create(outgoingHolder.LastID, incomingHolder, Buffer, Index, Length, MTU, Reliable);

			for (ushort i = 0; i < packet.SliceBuffers.Length; ++i)
				SendInternal(packet.SliceBuffers[i]);

			outgoingHolder.IncreaseLastID();
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
			ulong lastAckID = Buffer.ReadUInt64();
			uint ackMask = Buffer.ReadUInt32();
			byte[] ackMaskBytes = System.BitConverter.GetBytes(ackMask);
			bool isReliable = Buffer.ReadBool();
			ulong packetID = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			IncomingUDPPacketsHolder incomingHolder = (isReliable ? incomingReliablePacketHolder : incomingPacketHolder);

			IncomingUDPPacket packet = incomingHolder.GetPacket(packetID);
			if (packet == null)
			{
				packet = new IncomingUDPPacket(packetID, sliceCount, isReliable);
				incomingHolder.AddPacket(packet);
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
			{
				incomingHolder.SetLastID(packetID);

				if (isReliable)
				{
				}
				else
					HandleReceivedBuffer(packet.Combine());
			}
		}

		protected override BufferStream GetPingPacket()
		{
			BufferStream buffer = Packet.CreatePingBufferStream();



			return buffer;
		}
	}
}