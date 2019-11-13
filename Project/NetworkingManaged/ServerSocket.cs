// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
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

			public ServerSendCommand(Socket Socket, BufferStream Buffer, double SendTime) : base(Buffer, SendTime)
			{
				this.Socket = Socket;
			}
		}

		public delegate void ConnectionEventHandler(Client Client);
		public delegate void BufferReceivedEventHandler(Client Sender, BufferStream Buffer);

		private ClientList clients = null;

		public override bool IsReady
		{
			get { return Socket.IsBound; }
		}

		public override double Timestamp
		{
			get { return Time.CurrentEpochTime; }
		}

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
			Socket.ExclusiveAddressUse = true;

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
			Send(Target, Buffer, 0, (uint)Buffer.Length);
		}

		public virtual void Send(Client Target, byte[] Buffer, uint Length)
		{
			Send(Target, Buffer, 0, Length);
		}

		public virtual void Send(Client Target, byte[] Buffer, uint Index, uint Length)
		{
			BufferStream buffer = Constants.Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			Send(Target, buffer);
		}

		protected virtual void Send(Client Target, BufferStream Buffer)
		{
			AddSendCommand(new ServerSendCommand(Target.Socket, Buffer, Timestamp));
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

						BandwidthIn += (uint)size;

						uint index = 0;
						while (index != size)
						{
							uint packetSize = BitConverter.ToUInt32(ReceiveBuffer, (int)index);

							index += Constants.Packet.PACKET_SIZE_SIZE;

							HandleIncommingBuffer(client, new BufferStream(ReceiveBuffer, index, packetSize));

							index += packetSize;
						}
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

			double time = Time.CurrentEpochTime;

			Client.UpdateLastTouchTime(time);

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Constants.Packet.CreateIncommingBufferStream(Buffer.Buffer);

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
				double sendTime = Buffer.ReadFloat64();

				Client.UpdateLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = Constants.Packet.CreatePingBufferStream();

				Send(Client.Socket, pingBuffer);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			if (Timestamp < Command.SendTime + (LatencySimulation / 1000.0F))
				return false;

			ServerSendCommand sendCommand = (ServerSendCommand)Command;

			if (!SocketUtilities.IsSocketReady(sendCommand.Socket))
				return false;

			Send(sendCommand.Socket, Command.Buffer);

			return true;
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