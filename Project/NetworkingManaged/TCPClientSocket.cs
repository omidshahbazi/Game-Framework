// Copyright 2019. All Rights Reserved.
using System;
using System.Net;
using GameFramework.BinarySerializer;

namespace GameFramework.Networking
{
	public class TCPClientSocket : ClientSocket
	{
		public TCPClientSocket() : base(Protocols.TCP)
		{
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			Socket.BeginConnect(EndPoint, OnConnectedCallback, null);
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			HandleReceivedBuffer(Buffer);
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