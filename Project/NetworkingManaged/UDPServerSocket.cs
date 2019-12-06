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
			}

			public void SetIsConnected(bool Value)
			{
				IsConnected = Value;
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

		protected class UDPServerSendCommand : ServerSendCommand
		{
			public bool Reliable
			{
				get;
				private set;
			}

			public UDPServerSendCommand(Client Client, BufferStream Buffer, double SendTime, bool Reliable) : base(Client, Buffer, SendTime)
			{
				this.Reliable = Reliable;
			}
		}

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
			BufferStream buffer = Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			AddSendCommand(Target, buffer, Reliable);
		}

		protected virtual void AddSendCommand(Client Client, BufferStream Buffer, bool Reliable)
		{
			AddSendCommand(new UDPServerSendCommand(Client, Buffer, Timestamp, Reliable));
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

				AddSendCommand(Client, buffer, false);
			}
			else if (control == Constants.Control.PING)
			{
				if (!client.IsConnected)
					return;

				double sendTime = Buffer.ReadFloat64();

				Client.UpdateLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = Packet.CreatePingBufferStream();

				AddSendCommand(Client, pingBuffer, false);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			UDPServerSendCommand sendCommand = (UDPServerSendCommand)Command;
			UDPClient client = (UDPClient)sendCommand.Client;

			if (!client.IsReady)
				return false;

			//if (sendCommand.Reliable)
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