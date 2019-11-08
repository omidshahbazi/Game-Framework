﻿// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using System;
using System.Net;
using System.Net.Sockets;

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

		private class BufferReceivedvent : ServerEventBase
		{
			public BufferStream Buffer
			{
				get;
				private set;
			}

			public BufferReceivedvent(Client Client, BufferStream Buffer) : base(Client)
			{
				this.Buffer = Buffer;
			}
		}

		protected class ServerSendCommand : SendCommand
		{
			public Socket Socket
			{
				get;
				private set;
			}

			public ServerSendCommand(Socket Socket, BufferStream Buffer) : base(Buffer)
			{
				this.Socket = Socket;
			}
		}

		public delegate void ConnectionEventHandler(Client Client);
		public delegate void BufferReceivedEventHandler(Client Sender, BufferStream Buffer);

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
		public event BufferReceivedEventHandler OnBufferReceived = null;

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

		public void UnBind()
		{
			clients.Clear();

			Shutdown();
		}

		public void DisconnectClient(Client Client)
		{
			lock (clients)
				clients.Remove(Client);

			HandleClientDisconnection(Client);
		}

		public void Listen()
		{
			Socket.Listen((int)MaxConnection);

			RunReceiveThread();
			RunSenndThread();
		}

		public virtual void Send(Client Target, byte[] Buffer)
		{
			Send(Target, Buffer, 0, Buffer.Length);
		}

		public virtual void Send(Client Target, byte[] Buffer, int Length)
		{
			Send(Target, Buffer, 0, Length);
		}

		public virtual void Send(Client Target, byte[] Buffer, int Index, int Length)
		{
			BufferStream buffer = new BufferStream(new byte[Constants.Packet.HEADER_SIZE + Length]);
			buffer.Reset();
			buffer.WriteBytes(Constants.Control.BUFFER);
			buffer.WriteBytes(Buffer, Index, Length);

			Send(Target, buffer);
		}

		protected virtual void Send(Client Target, BufferStream Buffer)
		{
			AddSendCommand(new ServerSendCommand(Target.Socket, Buffer));
		}

		protected override void Receive()
		{
			if (!Socket.IsBound)
				return;

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
						if (client.Socket.Available == 0)
							continue;

						int size = client.Socket.Receive(ReceiveBuffer);

						BandwidthIn += (uint)size;

						HandleIncommingBuffer(client, new BufferStream(ReceiveBuffer, (uint)size));
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

		protected virtual void HandleIncommingBuffer(Client Client, BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.Packet.HEADER_SIZE, Buffer.Size - Constants.Packet.HEADER_SIZE);

				if (MultithreadedCallbacks)
				{
					if (OnBufferReceived != null)
						CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, Client, buffer);
				}
				else
				{
					AddEvent(new BufferReceivedvent(Client, buffer));
				}
			}
			else if (control == Constants.Control.PING)
			{
			}
		}

		protected override void HandleSendCommand(SendCommand Command)
		{
			ServerSendCommand sendCommand = (ServerSendCommand)Command;

			Send(sendCommand.Socket, Command.Buffer);
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

				SocketUtilities.CloseSocket(ev.Client.Socket);
			}
			else if (ev is BufferReceivedvent)
			{
				if (OnBufferReceived != null)
					CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, ev.Client, ((BufferReceivedvent)ev).Buffer);
			}
		}

		protected abstract void ProcessReceivedBuffer(Client Sender, BufferStream Buffer);

		protected void HandleReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			if (MultithreadedCallbacks)
			{
				if (OnBufferReceived != null)
					CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, Sender, Buffer);
			}
			else
			{
				AddEvent(new BufferReceivedvent(Sender, Buffer));
			}
		}

		private void HandleClientDisconnection(Client Client)
		{
			if (MultithreadedCallbacks)
			{
				if (OnClientDisconnected != null)
					CallbackUtilities.InvokeCallback(OnClientDisconnected.Invoke, Client);

				SocketUtilities.CloseSocket(Client.Socket);
			}
			else
			{
				AddEvent(new ClientDisconnectedEvent(Client));
			}
		}
	}
}