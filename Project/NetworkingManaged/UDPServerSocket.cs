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

			//public ulong LastIncommmingPacketID
			//{
			//	get;
			//	private set;
			//}

			public IncommingPacketMap IncommingPacketsMap
			{
				get;
				private set;
			}

			public ulong LastOutgoingPacketID
			{
				get;
				private set;
			}

			public uint MTU
			{
				get;
				private set;
			}

			public uint BandwidthInFromLastSecond
			{
				get;
				private set;
			}

			public UDPClient(IPEndPoint EndPoint)
			{
				endPoint = EndPoint;

				IncommingPacketsMap = new IncommingPacketMap();
			}

			public void SetIsConnected(bool Value)
			{
				IsConnected = Value;
			}

			//public void SetLastIncommingPacketID(ulong ID)
			//{
			//	LastIncommmingPacketID = ID;
			//}

			public IncommingRUDPPacket GetIncommingPacket(ulong ID)
			{
				if (IncommingPacketsMap.ContainsKey(ID))
					return IncommingPacketsMap[ID];

				return null;
			}

			public void AddIncommingPacket(IncommingRUDPPacket Packet)
			{
				IncommingPacketsMap[Packet.ID] = Packet;
			}

			public void IncreaseLastOutgoingPacketID()
			{
				++LastOutgoingPacketID;
			}

			public void UpdateMTU(uint MTU)
			{
				this.MTU = MTU;
			}

			public void AddBandwidthInFromLastSecond(uint Count)
			{
				BandwidthInFromLastSecond += Count;
			}

			public void ResetBandwidthInFromLastSecond()
			{
				BandwidthInFromLastSecond = 0;
			}
		}

		private class UDPClientList : List<UDPClient>
		{ }

		private class ClientMap : Dictionary<int, UDPClient>
		{ }

		private UDPClientList clients = null;
		private ClientMap clientsMap = null;

		private long lastBandwidthInCheck = 0;

		public override Client[] Clients
		{
			get
			{
				lock (clients)
					return clients.ToArray();
			}
		}

		public uint PacketRate
		{
			get;
			set;
		}

		public UDPServerSocket() : base(Protocols.UDP)
		{
			clients = new UDPClientList();
			clientsMap = new ClientMap();

			PacketRate = Constants.DEFAULT_PACKET_RATE;

			lastBandwidthInCheck = (long)Time.CurrentEpochTime;
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

			OutgoingRUDPPacket packet = OutgoingRUDPPacket.Create(client.LastOutgoingPacketID, Buffer, Index, Length, client.MTU, Reliable);

			for (ushort i = 0; i < packet.SliceBuffers.Length; ++i)
				SendInternal(Target, packet.SliceBuffers[i]);

			client.IncreaseLastOutgoingPacketID();
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

			if (Time.CurrentEpochTime - lastBandwidthInCheck >= 1)
			{
				lock (clients)
					for (int i = 0; i < clients.Count; ++i)
					{
						UDPClient client = clients[i];

						if (!client.IsConnected)
							continue;

						if (client.BandwidthInFromLastSecond <= PacketRate)
						{
							client.ResetBandwidthInFromLastSecond();

							continue;
						}

						int hash = GetIPEndPointHash(client.EndPoint);

						clients.Remove(client);
						clientsMap.Remove(hash);

						HandleClientDisconnection(client);
					}

				lastBandwidthInCheck = (long)Time.CurrentEpochTime;
			}
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
			bool isReliable = Buffer.ReadBool();
			ulong id = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			if (!isReliable && sliceCount == 1)
			{
				HandleReceivedBuffer(Sender, buffer);
				return;
			}

			UDPClient client = (UDPClient)Sender;

			IncommingRUDPPacket packet = client.GetIncommingPacket(id);
			if (packet == null)
			{
				packet = new IncommingRUDPPacket(id, sliceCount, isReliable);
				client.AddIncommingPacket(packet);
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
				HandleReceivedBuffer(Sender, packet.Combine());
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