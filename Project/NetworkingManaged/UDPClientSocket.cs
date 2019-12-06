// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System.Net;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		private ulong lastPacketID;

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
			OutgoingRUDPPacket packet = OutgoingRUDPPacket.Create(lastPacketID, Buffer, Index, Length, MTU, Reliable);

			for (ushort i = 0; i < packet.Buffers.Length; ++i)
				SendInternal(packet.Buffers[i]);

			++lastPacketID;
		}

		protected virtual void SendInternal(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer, Timestamp));
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.Connect(EndPoint);

			MTU = SocketUtilities.FindOptimumMTU(EndPoint.Address, Constants.UDP_MAX_MTU);
			MTU = 17;  //TODO: it's just a hack to test

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
			HandleReceivedBuffer(Buffer);
		}
	}
}