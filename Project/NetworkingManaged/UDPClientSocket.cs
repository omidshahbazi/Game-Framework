// Copyright 2019. All Rights Reserved.
using System.Net;
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		private class UDPSendCommand : SendCommand
		{
			public bool Reliable
			{
				get;
				private set;
			}

			public UDPSendCommand(BufferStream Buffer, double SendTimme, bool Reliable) : base(Buffer, SendTimme)
			{
				this.Reliable = Reliable;
			}
		}

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
			BufferStream buffer = Constants.Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			SendInternal(buffer, Reliable);
		}

		protected virtual void SendInternal(BufferStream Buffer, bool Reliable)
		{
			AddSendCommand(new UDPSendCommand(Buffer, Timestamp, Reliable));
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.Connect(EndPoint);

			MTU = SocketUtilities.FindOptimumMTU(EndPoint.Address, Constants.UDP_MAX_MTU);

			BufferStream buffer = Constants.Packet.CreateHandshakeBufferStream(MTU);
			AddSendCommand(new UDPSendCommand(buffer, Timestamp, false));

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
				BufferStream buffer = Constants.Packet.CreateIncommingBufferStream(Buffer.Buffer);

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

		protected override bool HandleSendCommand(SendCommand Command)
		{
			if (!SocketUtilities.GetIsReady(Socket))
				return false;

			UDPSendCommand sendCommand = (UDPSendCommand)Command;

			if (sendCommand.Reliable)
			{

			}

			SendOverSocket(Socket, sendCommand.Buffer);

			return true;
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			HandleReceivedBuffer(Buffer);
		}
	}
}