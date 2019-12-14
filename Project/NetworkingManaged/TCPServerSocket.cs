// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Networking
{
	public class TCPServerSocket : ServerSocket
	{
		private class TCPClient : Client
		{
			public override bool IsReady
			{
				get { return (SocketUtilities.GetIsReady(Socket) && base.IsReady); }
			}

			public Socket Socket
			{
				get;
				private set;
			}

			public override IPEndPoint EndPoint
			{
				get { return (IPEndPoint)Socket.RemoteEndPoint; }
			}

			public TCPClient(Socket Socket)
			{
				this.Socket = Socket;
			}
		}

		private class TCPClientList : List<TCPClient>
		{ }

		private TCPClientList clients = null;

		public uint MaxConnection
		{
			get;
			set;
		}

		public override Client[] Clients
		{
			get
			{
				lock (clients)
					return clients.ToArray();
			}
		}

		public TCPServerSocket(uint MaxConnection = 32) : base(Protocols.TCP)
		{
			clients = new TCPClientList();

			this.MaxConnection = MaxConnection;
		}

		public override void UnBind()
		{
			clients.Clear();

			base.UnBind();
		}

		public override void DisconnectClient(Client Client)
		{
			lock (clients)
				clients.Remove((TCPClient)Client);

			base.DisconnectClient(Client);
		}

		public override void Listen()
		{
			Socket.Listen((int)MaxConnection);

			base.Listen();
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
			BufferStream buffer = Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			SendInternal(Target, buffer);
		}

		protected virtual void SendInternal(Client Client, BufferStream Buffer)
		{
			AddSendCommand(new ServerSendCommand(Client, Buffer, Timestamp));
		}

		protected override void AcceptClients()
		{
			try
			{
				Socket clientSocket = Socket.Accept();

				TCPClient client = new TCPClient(clientSocket);

				lock (clients)
					clients.Add(client);

				RaiseOnClientConnected(client);
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode != SocketError.WouldBlock)
					throw e;
			}
		}

		protected override void ReadFromClients()
		{
			lock (clients)
			{
				TCPClientList disconnectedClients = new TCPClientList();

				for (int i = 0; i < clients.Count; ++i)
				{
					TCPClient client = clients[i];

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

						client.Statistics.AddRecievedPacketFromLastSecond();

						ProcessReceivedBuffer(client, (uint)size);
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

		protected override void HandleIncommingBuffer(Client Client, BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			double time = Time.CurrentEpochTime;

			Client.Statistics.SetLastTouchTime(time);

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Packet.CreateIncommingBufferStream(Buffer.Buffer);

				ProcessReceivedBuffer(Client, buffer);
			}
			else if (control == Constants.Control.PING)
			{
				double sendTime = Buffer.ReadFloat64();

				Client.Statistics.SetLatency((uint)((time - sendTime) * 1000));

				BufferStream pingBuffer = Packet.CreatePingBufferStream();

				SendInternal(Client, pingBuffer);
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			ServerSendCommand sendCommand = (ServerSendCommand)Command;
			TCPClient client = (TCPClient)sendCommand.Client;

			if (!client.IsReady)
				return false;

			client.Statistics.AddBandwidthIn(Command.Buffer.Size);

			SendOverSocket(client.Socket, Command.Buffer);

			return true;
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			HandleReceivedBuffer(Sender, Buffer);
		}

		protected override void CloseClientConnection(Client Client)
		{
			base.CloseClientConnection(Client);

			SocketUtilities.CloseSocket(((TCPClient)Client).Socket);
		}
	}
}