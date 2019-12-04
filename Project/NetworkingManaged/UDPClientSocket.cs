// Copyright 2019. All Rights Reserved.
using System.Net;
using GameFramework.BinarySerializer;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		public UDPClientSocket() : base(Protocols.UDP)
		{
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.Connect(EndPoint);

			BufferStream buffer = Constants.Packet.CreateConnectionBufferStream();
			AddSendCommand(new SendCommand(buffer, Timestamp));

			RunReceiveThread();
			RunSenndThread();

			//IsConnected = true;

			//RaiseOnConnectedEvent();

			SocketUtilities.FindOptimumMTU(IPAddress.Parse("8.8.8.8"), 2000);
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			HandleReceivedBuffer(Buffer);
		}
	}
}