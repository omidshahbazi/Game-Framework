// Copyright 2019. All Rights Reserved.
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameFramework.NetworkingManaged
{
	public abstract class ServerSocket : BaseSocket
	{
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

		public event ConnectionEventHandler OnClientConnected = null;
		public event ConnectionEventHandler OnClientDisconnected = null;

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
						OnClientConnected(client);
				}
				else
				{
					// call OnClientConnected on main thread
				}
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode != SocketError.WouldBlock)
					throw e;
			}

			lock (clients)
			{
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
							if (MultithreadedCallbacks)
							{
								if (OnClientDisconnected != null)
									OnClientDisconnected(client);
							}
							else
							{
								// call OnClientDisconnected on main thread
							}

							continue;
						}
						else
							throw e;
					}
				}
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