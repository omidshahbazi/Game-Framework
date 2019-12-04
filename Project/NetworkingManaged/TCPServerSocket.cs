// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Networking
{
	public class TCPServerSocket : ServerSocket
	{
		private class TCPClient : Client
		{
			public override bool IsReady
			{
				get { return (SocketUtilities.GetIsReady(Socket) && base.IsReady); }
			}

			public Socket Socket
			{
				get;
				private set;
			}

			public override IPEndPoint EndPoint
			{
				get { return (IPEndPoint)Socket.RemoteEndPoint; }
			}

			public TCPClient(Socket Socket)
			{
				this.Socket = Socket;
			}
		}

		private class TCPClientList : List<TCPClient>
		{ }

		private TCPClientList clients = null;

		public uint MaxConnection
		{
			get;
			set;
		}

		public override Client[] Clients
		{
			get
			{
				lock (clients)
					return clients.ToArray();
			}
		}

		public TCPServerSocket(uint MaxConnection = 32) : base(Protocols.TCP)
		{
			clients = new TCPClientList();

			this.MaxConnection = MaxConnection;
		}

		public override void UnBind()
		{
			clients.Clear();

			base.UnBind();
		}

		public override void DisconnectClient(Client Client)
		{
			lock (clients)
				clients.Remove((TCPClient)Client);

			base.DisconnectClient(Client);
		}

		public override void Listen()
		{
			Socket.Listen((int)MaxConnection);

			base.Listen();
		}

		protected override void SendOverSocket(Client Client, BufferStream Buffer)
		{
			SendOverSocket(((TCPClient)Client).Socket, Buffer);
		}

		protected override void AcceptClients()
		{
			try
			{
				Socket clientSocket = Socket.Accept();

				TCPClient client = new TCPClient(clientSocket);

				lock (clients)
					clients.Add(client);

				RaiseOnClientConnected(client);
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode != SocketError.WouldBlock)
					throw e;
			}
		}

		protected override void ReadFromClients()
		{
			lock (clients)
			{
				TCPClientList disconnectedClients = new TCPClientList();

				for (int i = 0; i < clients.Count; ++i)
				{
					TCPClient client = clients[i];

					try
					{
						int size = 0;

						lock (Socket)
						{
							if (client.Socket.Available == 0)
							{
								if (!client.IsReady)
								{
									disconnectedClients.Add(client);

									HandleClientDisconnection(client);
								}

								continue;
							}

							size = client.Socket.Receive(ReceiveBuffer);
						}

						ProcessReceivedBuffer(client, (uint)size);
					}
					catch (SocketException e)
					{
						if (e.SocketErrorCode == SocketError.WouldBlock)
							continue;
						else if (e.SocketErrorCode == SocketError.ConnectionReset)
						{
							disconnectedClients.Add(client);

							HandleClientDisconnection(client);

							continue;
						}

						throw e;
					}
					catch (Exception e)
					{
						throw e;
					}
				}

				for (int i = 0; i < disconnectedClients.Count; ++i)
					clients.Remove(disconnectedClients[i]);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			if (Timestamp < Command.SendTime + (LatencySimulation / 1000.0F))
				return false;

			ServerSendCommand sendCommand = (ServerSendCommand)Command;
			TCPClient client = (TCPClient)sendCommand.Client;

			if (!SocketUtilities.GetIsReady(client.Socket))
				return false;

			SendOverSocket(client.Socket, Command.Buffer);

			return true;
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			HandleReceivedBuffer(Sender, Buffer);
		}

		protected override void CloseClientConnection(Client Client)
		{
			base.CloseClientConnection(Client);

			SocketUtilities.CloseSocket(((TCPClient)Client).Socket);
		}
	}
}