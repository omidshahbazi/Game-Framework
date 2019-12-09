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

			public bool IsConnected
			{
				get;
				private set;
			}

			public IncomingUDPPacketsHolder IncomingReliablePacketHolder
			{
				get;
				private set;
			}

			public IncomingUDPPacketsHolder IncomingPacketHolder
			{
				get;
				private set;
			}

			public OutgoingUDPPacketsHolder OutgoingReliablePacketHolder
			{
				get;
				private set;
			}

			public OutgoingUDPPacketsHolder OutgoingPacketHolder
			{
				get;
				private set;
			}

			public uint MTU
			{
				get;
				private set;
			}

			public UDPClient(IPEndPoint EndPoint)
			{
				endPoint = EndPoint;

				IncomingReliablePacketHolder = new IncomingUDPPacketsHolder();
				IncomingPacketHolder = new IncomingUDPPacketsHolder();
				OutgoingReliablePacketHolder = new OutgoingUDPPacketsHolder();
				OutgoingPacketHolder = new OutgoingUDPPacketsHolder();
			}

			public void SetIsConnected(bool Value)
			{
				IsConnected = Value;
			}

			public void UpdateMTU(uint MTU)
			{
				this.MTU = MTU;
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
			UDPClient client = (UDPClient)Target;

			OutgoingUDPPacketsHolder outgoingHolder = (Reliable ? client.OutgoingReliablePacketHolder : client.OutgoingPacketHolder);
			IncomingUDPPacketsHolder incomingHolder = (Reliable ? client.IncomingReliablePacketHolder : client.IncomingPacketHolder);

			OutgoingUDPPacket packet = OutgoingUDPPacket.Create(outgoingHolder.LastID, incomingHolder, Buffer, Index, Length, client.MTU, Reliable);

			for (ushort i = 0; i < packet.SliceBuffers.Length; ++i)
				SendInternal(Target, packet.SliceBuffers[i]);

			outgoingHolder.IncreaseLastID();
		}

		protected virtual void SendInternal(Client Client, BufferStream Buffer)
		{
			AddSendCommand(new ServerSendCommand(Client, Buffer, Timestamp));
		}

		protected override void AcceptClients()
		{
		}

		protected override void ReadFromClients()
		{
			ReadFromSocket();

			CheckClientsConnection();
		}

		protected override void HandleIncommingBuffer(Client Client, BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			double time = Time.CurrentEpochTime;

			Client.UpdateLastTouchTime(time);

			UDPClient client = (UDPClient)Client;

			if (control == Constants.Control.BUFFER)
			{
				if (!client.IsConnected)
					return;

				BufferStream buffer = Packet.CreateIncommingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(Client, buffer);
			}
			else if (control == Constants.Control.HANDSHAKE)
			{
				client.SetIsConnected(true);

				uint mtu = Buffer.ReadUInt32();
				client.UpdateMTU(mtu);

				lock (clients)
					clients.Add(client);

				BufferStream buffer = Packet.CreateHandshakeBackBufferStream(PacketRate);

				SendInternal(Client, buffer);
			}
			else if (control == Constants.Control.PING)
			{
				if (!client.IsConnected)
					return;

				double sendTime = Buffer.ReadFloat64();

				Client.UpdateLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = Packet.CreatePingBufferStream();

				SendInternal(Client, pingBuffer);
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
			ulong lastAckID = Buffer.ReadUInt64();
			bool isReliable = Buffer.ReadBool();
			ulong packetID = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			UDPClient client = (UDPClient)Sender;

			OutgoingUDPPacketsHolder outgoingHolder = (isReliable ? client.OutgoingReliablePacketHolder : client.OutgoingPacketHolder);

			if (!isReliable && sliceCount == 1)
			{
				outgoingHolder.SetLastID(lastAckID);

				HandleReceivedBuffer(Sender, buffer);

				return;
			}

			IncomingUDPPacketsHolder incomingHolder = (isReliable ? client.IncomingReliablePacketHolder : client.IncomingPacketHolder);

			IncomingUDPPacket packet = incomingHolder.GetPacket(packetID);
			if (packet == null)
			{
				packet = new IncomingUDPPacket(packetID, sliceCount, isReliable);
				incomingHolder.AddPacket(packet);
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
			{
				outgoingHolder.SetLastID(lastAckID);

				if (isReliable)
				{
					ulong prevID = 0;

					var it = incomingHolder.PacketsMap.GetEnumerator();
					while (it.MoveNext())
					{
						//if (it.Current.Key)
						HandleReceivedBuffer(Sender, packet.Combine());
					}
				}
				else
					HandleReceivedBuffer(Sender, packet.Combine());
			}
		}

		private void ReadFromSocket()
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

				client.AddBandwidthInFromLastSecond((uint)size);

				ProcessReceivedBuffer(client, (uint)size);
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.WouldBlock)
					return;
				else if (e.SocketErrorCode == SocketError.ConnectionReset)
				{
					if (ipEndPoint.Address == IPAddress.IPv6Any)
						return;

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

		private void CheckClientsConnection()
		{
			for (int i = 0; i < clients.Count; ++i)
			{
				UDPClient client = clients[i];

				if (client.IsReady)
					continue;

				int hash = GetIPEndPointHash(client.EndPoint);

				clients.Remove(client);
				clientsMap.Remove(hash);

				HandleClientDisconnection(client);
			}
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