// Copyright 2019. All Rights Reserved.
using System;
using System.Net;
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;

namespace GameFramework.Networking
{
	public class TCPClientSocket : ClientSocket
	{
		public TCPClientSocket() : base(Protocols.TCP)
		{
		}

		public virtual void Send(byte[] Buffer)
		{
			Send(Buffer, 0, (uint)Buffer.Length);
		}

		public virtual void Send(byte[] Buffer, uint Length)
		{
			Send(Buffer, 0, Length);
		}

		public virtual void Send(byte[] Buffer, uint Index, uint Length)
		{
			BufferStream buffer = Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			SendInternal(buffer);
		}

		protected virtual void SendInternal(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer, Timestamp));
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.BeginConnect(EndPoint, OnConnectedCallback, null);
		}

		protected override void Receive()
		{
			if (!IsConnected)
				return;

			base.Receive();
		}

		protected override void HandleIncommingBuffer(BufferStream Buffer)
		{
			double time = Time.CurrentEpochTime;

			LastTouchTime = time;

			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Packet.CreateIncommingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(buffer);
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

		protected override BufferStream GetPingPacket()
		{
			return Packet.CreatePingBufferStream();
		}

		private void OnConnectedCallback(IAsyncResult Result)
		{
			if (Socket.Connected)
			{
				lock (Socket)
				{
					Socket.EndConnect(Result);
				}

				IsConnected = true;

				RunReceiveThread();
				RunSenndThread();

				RaiseOnConnectedEvent();
			}
			else
			{
				RaiseOnConnectionFailedEvent();

				IsConnected = false;
			}
		}
	}
}