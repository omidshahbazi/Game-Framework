// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Networking
{
	public class UDPServerSocket : ServerSocket
	{
		private class UDPClient : Client
		{
			private IPEndPoint endPoint;

			public override IPEndPoint EndPoint
			{
				get { return endPoint; }
			}

			public UDPClient(IPEndPoint EndPoint)
			{
				endPoint = EndPoint;
			}
		}

		private class UDPClientList : List<UDPClient>
		{ }

		private UDPClientList clients = null;

		public override Client[] Clients
		{
			get
			{
				lock (clients)
					return clients.ToArray();
			}
		}

		public UDPServerSocket() : base(Protocols.UDP)
		{
			clients = new UDPClientList();
		}

		protected override void SendOverSocket(Client Client, BufferStream Buffer)
		{
		}

		protected override void AcceptClients()
		{
		}

		protected override void ReadFromClients()
		{
			try
			{
				int size = 0;
				EndPoint endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

				lock (Socket)
				{
					if (Socket.Available == 0)
						return;

					
					size = Socket.ReceiveFrom(ReceiveBuffer, ref endPoint);
				}

				//ProcessReceivedBuffer(client, (uint)size);
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.WouldBlock)
					return;

				throw e;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			return false;
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
		}
	}
}