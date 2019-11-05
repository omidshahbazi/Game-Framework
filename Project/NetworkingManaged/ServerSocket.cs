// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameFramework.NetworkingManaged
{
	public abstract class ServerSocket : BaseSocket
	{
		private abstract class ServerEventBase : EventBase
		{
			public Client Client
			{
				get;
				private set;
			}

			public ServerEventBase(Client Client)
			{
				this.Client = Client;
			}
		}

		private class ClientConnectedEvent : ServerEventBase
		{
			public ClientConnectedEvent(Client Client) : base(Client)
			{ }
		}

		private class ClientDisconnectedEvent : ServerEventBase
		{
			public ClientDisconnectedEvent(Client Client) : base(Client)
			{ }
		}

		public delegate void ConnectionEventHandler(Client Client);

		private Thread receiveThread = null;
		private ClientList clients = null;

		public uint MaxConnection
		{
			get;
			set;
		}

		public Client[] Clients
		{
			get
			{
				lock (clients)
					return clients.ToArray();
			}
		}

		public event ConnectionEventHandler OnClientConnected = null;
		public event ConnectionEventHandler OnClientDisconnected = null;

		public ServerSocket(Protocols Type, uint MaxConnection) : base(Type)
		{
			clients = new ClientList();

			this.MaxConnection = MaxConnection;
		}

		public void Bind(string Host, ushort Port)
		{
			Bind(SocketUtilities.ResolveDomain(Host), Port);
		}

		public void Bind(IPAddress IP, ushort Port)
		{
			Bind(new IPEndPoint(IP, Port));
		}

		public void Bind(IPEndPoint EndPoint)
		{
			if (EndPoint.AddressFamily == AddressFamily.InterNetwork)
				EndPoint.Address = SocketUtilities.MapIPv4ToIPv6(EndPoint.Address);

			Socket.Bind(EndPoint);
		}

		public void Listen()
		{
			Socket.Listen((int)MaxConnection);

			if (MultithreadedReceive)
			{
				receiveThread = new Thread(ListenWorker);
				receiveThread.Start();
			}
		}

		protected override void Receive()
		{
			try
			{
				Socket clientSocket = Socket.Accept();

				Client client = new Client(clientSocket);

				lock (clients)
					clients.Add(client);

				if (MultithreadedCallbacks)
				{
					if (OnClientConnected != null)
						CallbackUtilities.InvokeCallback(OnClientConnected.Invoke, client);
				}
				else
				{
					AddEvent(new ClientConnectedEvent(client));
				}
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode != SocketError.WouldBlock)
					throw e;
			}

			lock (clients)
			{
				ClientList disconnectedClients = new ClientList();

				for (int i = 0; i < clients.Count; ++i)
				{
					Client client = clients[i];

					try
					{
						int len = client.Socket.Receive(ReceiveBuffer);
					}
					catch (SocketException e)
					{
						if (e.SocketErrorCode == SocketError.WouldBlock)
							continue;
						else if (e.SocketErrorCode == SocketError.ConnectionReset)
						{
							disconnectedClients.Add(client);

							if (MultithreadedCallbacks)
							{
								if (OnClientDisconnected != null)
									CallbackUtilities.InvokeCallback(OnClientDisconnected.Invoke, client);
							}
							else
							{
								AddEvent(new ClientDisconnectedEvent(client));
							}

							continue;
						}
						else
							throw e;
					}
				}

				for (int i = 0; i < disconnectedClients.Count; ++i)
					clients.Remove(disconnectedClients[i]);
			}
		}

		protected override void ProcessEvent(EventBase Event)
		{
			ServerEventBase ev = (ServerEventBase)Event;

			if (ev is ClientConnectedEvent)
			{
				if (OnClientConnected != null)
					CallbackUtilities.InvokeCallback(OnClientConnected.Invoke, ev.Client);
			}
			else if (ev is ClientDisconnectedEvent)
			{
				if (OnClientDisconnected != null)
					CallbackUtilities.InvokeCallback(OnClientDisconnected.Invoke, ev.Client);
			}
		}

		private void ListenWorker()
		{
			while (Socket.IsBound)
			{
				Thread.Sleep(1);

				Receive();
			}
		}
	}
}