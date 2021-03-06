﻿// Copyright 2019. All Rights Reserved.
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

			public IncomingUDPPacketsHolder IncomingNonReliablePacketHolder
			{
				get;
				private set;
			}

			public OutgoingUDPPacketsHolder OutgoingReliablePacketHolder
			{
				get;
				private set;
			}

			public OutgoingUDPPacketsHolder OutgoingNonReliablePacketHolder
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
				IncomingNonReliablePacketHolder = new IncomingUDPPacketsHolder();
				OutgoingReliablePacketHolder = new OutgoingUDPPacketsHolder();
				OutgoingNonReliablePacketHolder = new OutgoingUDPPacketsHolder();
			}

			public void SetIsConnected(bool Value)
			{
				IsConnected = Value;
			}

			public void SetMTU(uint MTU)
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

			OutgoingUDPPacketsHolder outgoingHolder = (Reliable ? client.OutgoingReliablePacketHolder : client.OutgoingNonReliablePacketHolder);
			IncomingUDPPacketsHolder incomingHolder = (Reliable ? client.IncomingReliablePacketHolder : client.IncomingNonReliablePacketHolder);

			OutgoingUDPPacket packet = OutgoingUDPPacket.CreateOutgoingBufferStream(outgoingHolder, incomingHolder, Buffer, Index, Length, client.MTU, Reliable);

			SendPacket(Target, packet);
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

		protected override void HandleIncomingBuffer(Client Client, BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			double time = Time.CurrentEpochTime;

			Client.Statistics.SetLastTouchTime(time);

			UDPClient client = (UDPClient)Client;

			if (control == Constants.Control.BUFFER)
			{
				if (!client.IsConnected)
					return;

				BufferStream buffer = Packet.CreateIncomingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(Client, buffer);
			}
			else if (control == Constants.Control.HANDSHAKE)
			{
				client.SetIsConnected(true);

				uint mtu = Buffer.ReadUInt32();
				client.SetMTU(mtu);

				lock (clients)
					clients.Add(client);

				BufferStream buffer = Packet.CreateHandshakeBackBufferStream(client.Statistics.PacketCountRate);

				SendInternal(Client, buffer);

				RaiseOnClientConnected(client);
			}
			else if (control == Constants.Control.PING)
			{
				if (!client.IsConnected)
					return;

				double sendTime = Buffer.ReadFloat64();

				Client.Statistics.SetLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = OutgoingUDPPacket.CreatePingBufferStream(client.OutgoingReliablePacketHolder, client.IncomingReliablePacketHolder, client.OutgoingNonReliablePacketHolder, client.IncomingNonReliablePacketHolder);

				SendInternal(Client, pingBuffer);

				ulong lastAckID = Buffer.ReadUInt64();
				uint ackMask = Buffer.ReadUInt32();
				client.OutgoingReliablePacketHolder.SetLastAckID(lastAckID);
				client.OutgoingReliablePacketHolder.SetAckMask(ackMask);

				lastAckID = Buffer.ReadUInt64();
				ackMask = Buffer.ReadUInt32();
				client.OutgoingNonReliablePacketHolder.SetLastAckID(lastAckID);
				client.OutgoingNonReliablePacketHolder.SetAckMask(ackMask);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			ServerSendCommand sendCommand = (ServerSendCommand)Command;
			UDPClient client = (UDPClient)sendCommand.Client;

			if (!client.IsReady)
				return false;

			client.Statistics.AddBandwidthOut(Command.Buffer.Size);

			SendOverSocket(client.EndPoint, Command.Buffer);

			return true;
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			ulong lastAckID = Buffer.ReadUInt64();
			uint ackMask = Buffer.ReadUInt32();
			bool isReliable = Buffer.ReadBool();
			ulong packetID = Buffer.ReadUInt64();
			ushort sliceCount = Buffer.ReadUInt16();
			ushort sliceIndex = Buffer.ReadUInt16();

			BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

			UDPClient client = (UDPClient)Sender;

			IncomingUDPPacketsHolder incomingHolder = (isReliable ? client.IncomingReliablePacketHolder : client.IncomingNonReliablePacketHolder);

			IncomingUDPPacket packet = incomingHolder.GetPacket(packetID);
			if (packet == null)
			{
				packet = new IncomingUDPPacket(packetID, sliceCount);
				incomingHolder.AddPacket(packet);
			}

			packet.SetSliceBuffer(sliceIndex, buffer);

			if (packet.IsCompleted)
			{
				if (incomingHolder.LastID < packet.ID)
					incomingHolder.SetLastID(packet.ID);

				if (isReliable)
					ProcessIncomingReliablePackets(client);
				else
					ProcessIncomingNonReliablePacket(client, packet);
			}

			OutgoingUDPPacketsHolder outgoingHolder = (isReliable ? client.OutgoingReliablePacketHolder : client.OutgoingNonReliablePacketHolder);
			outgoingHolder.SetLastAckID(lastAckID);
			outgoingHolder.SetAckMask(ackMask);

			if (isReliable)
				ProcessOutgoingReliablePackets(client);
			else
				ProcessOutgoingNonReliablePackets(client);
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

				client.Statistics.AddReceivedPacketFromLastSecond();

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

		private void SendPacket(Client Target, OutgoingUDPPacket Packet)
		{
			for (ushort i = 0; i < Packet.SliceBuffers.Length; ++i)
				SendInternal(Target, Packet.SliceBuffers[i]);
		}

		private void ProcessIncomingReliablePackets(UDPClient Sender)
		{
			IncomingUDPPacketsHolder.ProcessReliablePackets(Sender.IncomingReliablePacketHolder, (Buffer) =>
			{
				HandleReceivedBuffer(Sender, Buffer);
			});
		}

		private void ProcessIncomingNonReliablePacket(UDPClient Sender, IncomingUDPPacket Packet)
		{
			IncomingUDPPacketsHolder.ProcessNonReliablePacket(Sender.IncomingNonReliablePacketHolder, Packet, (Buffer) =>
			{
				HandleReceivedBuffer(Sender, Buffer);
			});
		}

		private void ProcessOutgoingReliablePackets(UDPClient Target)
		{
			OutgoingUDPPacketsHolder.ProcessReliablePackets(Target.OutgoingReliablePacketHolder, (Packet) =>
			{
				SendPacket(Target, Packet);
			});
		}

		private void ProcessOutgoingNonReliablePackets(UDPClient Target)
		{
			OutgoingUDPPacketsHolder.ProcessNonReliablePackets(Target.OutgoingNonReliablePacketHolder, (Packet) =>
			{
				SendPacket(Target, Packet);
			});
		}

		private UDPClient GetOrAddClient(IPEndPoint EndPoint)
		{
			int hash = GetIPEndPointHash(EndPoint);

			if (clientsMap.ContainsKey(hash))
				return clientsMap[hash];

			UDPClient client = new UDPClient(EndPoint);
			client.Statistics.SetPacketCountRate(PacketCountRate);

			clientsMap[hash] = client;

			return client;
		}

		private static int GetIPEndPointHash(IPEndPoint EndPoint)
		{
			return EndPoint.GetHashCode();
		}
	}
}