// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Networking
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
			public Client Client
			{
				get;
				private set;
			}

			public ServerSendCommand(Client Client, BufferStream Buffer, double SendTime) : base(Buffer, SendTime)
			{
				this.Client = Client;
			}
		}

		private long lastBandwidthInCheck = 0;

		public delegate void ConnectionEventHandler(Client Client);
		public delegate void BufferReceivedEventHandler(Client Sender, BufferStream Buffer);

		public override bool IsReady
		{
			get { return Socket.IsBound; }
		}

		public override double Timestamp
		{
			get { return Time.CurrentEpochTime; }
		}

		public abstract Client[] Clients
		{
			get;
		}

		public uint PacketCountRate
		{
			get;
			set;
		}

		public event ConnectionEventHandler OnClientConnected = null;
		public event ConnectionEventHandler OnClientDisconnected = null;
		public event BufferReceivedEventHandler OnBufferReceived = null;

		public ServerSocket(Protocols Type) : base(Type)
		{
			lastBandwidthInCheck = (long)Time.CurrentEpochTime;

			PacketCountRate = Constants.DEFAULT_PACKET_COUNT_RATE;
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

		public virtual void UnBind()
		{
			Shutdown();
		}

		public virtual void DisconnectClient(Client Client)
		{
			HandleClientDisconnection(Client);
		}

		public virtual void Listen()
		{
			RunReceiveThread();
			RunSenndThread();
		}

		protected override void Receive()
		{
			AcceptClients();

			ReadFromClients();

			CheckClientsFlow();
		}

		protected abstract void AcceptClients();

		protected abstract void ReadFromClients();

		protected void ProcessReceivedBuffer(Client Client, uint Size)
		{
			Statistics.AddBandwidthIn(Size);
			Client.Statistics.AddBandwidthIn(Size);

			uint index = 0;
			while (index < Size)
			{
				uint packetSize = BitConverter.ToUInt32(ReceiveBuffer, (int)index);

				index += Packet.PACKET_SIZE_SIZE;

				HandleIncomingBuffer(Client, new BufferStream(ReceiveBuffer, index, packetSize));

				index += packetSize;
			}
		}

		protected abstract void HandleIncomingBuffer(Client Client, BufferStream Buffer);

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

				CloseClientConnection(ev.Client);
			}
			else if (ev is BufferReceivedvent)
			{
				CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, ev.Client, ((BufferReceivedvent)ev).Buffer);
			}
		}

		protected abstract void ProcessReceivedBuffer(Client Sender, BufferStream Buffer);

		protected void HandleReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			if (OnBufferReceived != null)
			{
				CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, Sender, Buffer);
			}
			else
			{
				AddEvent(new BufferReceivedvent(Sender, Buffer));
			}
		}

		protected virtual void CloseClientConnection(Client Client)
		{

		}

		protected void HandleClientDisconnection(Client Client)
		{
			if (MultithreadedCallbacks)
			{
				if (OnClientDisconnected != null)
					CallbackUtilities.InvokeCallback(OnClientDisconnected.Invoke, Client);

				CloseClientConnection(Client);
			}
			else
			{
				AddEvent(new ClientDisconnectedEvent(Client));
			}
		}

		protected void RaiseOnClientConnected(Client Client)
		{
			if (MultithreadedCallbacks)
			{
				if (OnClientConnected != null)
					CallbackUtilities.InvokeCallback(OnClientConnected.Invoke, Client);
			}
			else
			{
				AddEvent(new ClientConnectedEvent(Client));
			}
		}

		private void CheckClientsFlow()
		{
			if (Time.CurrentEpochTime - lastBandwidthInCheck < 1)
				return;

			Client[] clients = Clients;

			for (int i = 0; i < clients.Length; ++i)
			{
				Client client = clients[i];

				if (client.Statistics.ReceivedPacketFromLastSecond <= PacketCountRate)
				{
					client.Statistics.ResetReceivedPacketFromLastSecond();

					continue;
				}

				//int hash = GetIPEndPointHash(client.EndPoint);

				//clients.Remove(client);
				//clientsMap.Remove(hash);

				//HandleClientDisconnection(client);
			}

			lastBandwidthInCheck = (long)Time.CurrentEpochTime;
		}
	}
}