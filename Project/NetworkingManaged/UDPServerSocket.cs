// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
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

		private class ClientMap : Dictionary<int, UDPClient>
		{ }

		private UDPClientList clients = null;
		private ClientMap clientsMap = null;

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
			clientsMap = new ClientMap();
		}

		public virtual void Send(Client Target, byte[] Buffer, bool Reliable = true)
		{
			Send(Target, Buffer, 0, (uint)Buffer.Length, Reliable);
		}

		public virtual void Send(Client Target, byte[] Buffer, uint Length, bool Reliable = true)
		{
			Send(Target, Buffer, 0, Length, Reliable);
		}

		public virtual void Send(Client Target, byte[] Buffer, uint Index, uint Length, bool Reliable = true)
		{
			BufferStream buffer = Constants.Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			AddSendCommand(Target, buffer, Reliable);
		}

		protected virtual void AddSendCommand(Client Client, BufferStream Buffer, bool Reliable)
		{
			AddSendCommand(new ServerSendCommand(Client, Buffer, Timestamp));
		}

		protected override void AcceptClients()
		{
		}

		protected override void ReadFromClients()
		{
			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

			try
			{
				int size = 0;
				EndPoint endPoint = ipEndPoint;

				lock (Socket)
				{
					if (Socket.Available == 0)
						return;

					size = Socket.ReceiveFrom(ReceiveBuffer, ref endPoint);

					ipEndPoint = (IPEndPoint)endPoint;
				}

				UDPClient client = GetOrAddClient(ipEndPoint);

				lock (clients)
					clients.Add(client);

				ProcessReceivedBuffer(client, (uint)size);
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.WouldBlock)
					return;
				else if (e.SocketErrorCode == SocketError.ConnectionReset)
				{
					int hash = GetIPEndPointHash(ipEndPoint);

					lock (clients)
					{
						UDPClient client = GetOrAddClient(ipEndPoint);

						clients.Remove(client);
						clientsMap.Remove(hash);

						HandleClientDisconnection(client);
					}

					return;
				}

				throw e;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		protected override void HandleIncommingBuffer(Client Client, BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			double time = Time.CurrentEpochTime;

			Client.UpdateLastTouchTime(time);

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Constants.Packet.CreateIncommingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(Client, buffer);
			}
			else if (control == Constants.Control.CONNECTION)
			{
				BufferStream buffer = Constants.Packet.CreateConnectionBufferStream();

				AddSendCommand(Client, buffer, false);
			}
			else if (control == Constants.Control.PING)
			{
				double sendTime = Buffer.ReadFloat64();

				Client.UpdateLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = Constants.Packet.CreatePingBufferStream();

				AddSendCommand(Client, pingBuffer, false);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			ServerSendCommand sendCommand = (ServerSendCommand)Command;
			UDPClient client = (UDPClient)sendCommand.Client;

			if (!client.IsReady)
				return false;

			SendOverSocket(client.EndPoint, Command.Buffer);

			return true;
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			HandleReceivedBuffer(Sender, Buffer);
		}

		private UDPClient GetOrAddClient(IPEndPoint EndPoint)
		{
			int hash = GetIPEndPointHash(EndPoint);

			if (clientsMap.ContainsKey(hash))
				return clientsMap[hash];

			UDPClient client = new UDPClient(EndPoint);

			clientsMap[hash] = client;

			return client;
		}

		private static int GetIPEndPointHash(IPEndPoint EndPoint)
		{
			return EndPoint.GetHashCode();
		}
	}
}