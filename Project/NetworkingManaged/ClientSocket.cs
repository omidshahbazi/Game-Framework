﻿// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.Networking
{
	public abstract class ClientSocket : BaseSocket
	{
		private abstract class ClientEventBase : EventBase
		{
		}

		private class ConnectedEvent : ClientEventBase
		{
		}

		private class ConnectionFailedEvent : ClientEventBase
		{
		}

		private class DisconnectedEvent : ClientEventBase
		{
		}

		private class BufferReceivedvent : ClientEventBase
		{
			public BufferStream Buffer
			{
				get;
				private set;
			}

			public BufferReceivedvent(BufferStream Buffer)
			{
				this.Buffer = Buffer;
			}
		}

		public delegate void ConnectionEventHandler();
		public delegate void BufferReceivedEventHandler(BufferStream Buffer);

		private double lastPingTime = 0;
		private double timeOffset = 0;

		public bool IsConnected
		{
			get;
			protected set;
		}

		public override bool IsReady
		{
			get { return SocketUtilities.GetIsReady(Socket); }
		}

		public override double Timestamp
		{
			get { return Time.CurrentEpochTime + timeOffset; }
		}

		public double LastTouchTime
		{
			get;
			private set;
		}

		public uint Latency
		{
			get;
			private set;
		}

		public event ConnectionEventHandler OnConnected = null;
		public event ConnectionEventHandler OnConnectionFailed = null;
		public event ConnectionEventHandler OnDisconnected = null;
		public event BufferReceivedEventHandler OnBufferReceived = null;

		public ClientSocket(Protocols Type) : base(Type)
		{
		}

		public override void Service()
		{
			if (IsConnected && LastTouchTime + Constants.PING_TIME <= Time.CurrentEpochTime)
			{
				LastTouchTime = Time.CurrentEpochTime;

				SendPing();
			}

			base.Service();
		}

		public void Connect(string Host, ushort Port)
		{
			Connect(SocketUtilities.ResolveDomain(Host), Port);
		}

		public void Connect(IPAddress IP, ushort Port)
		{
			Connect(new IPEndPoint(IP, Port));
		}

		public void Connect(IPEndPoint EndPoint)
		{
			if (EndPoint.AddressFamily == AddressFamily.InterNetwork)
				EndPoint.Address = SocketUtilities.MapIPv4ToIPv6(EndPoint.Address);

			ConnectInternal(EndPoint);
		}

		public void Disconnect()
		{
			Shutdown();
		}

		public virtual void Send(byte[] Buffer)
		{
			Send(Buffer, 0, (uint)Buffer.Length);
		}

		public virtual void Send(byte[] Buffer, uint Length)
		{
			Send(Buffer, 0, Length);
		}

		public virtual void Send(byte[] Buffer, uint Index, uint Length)
		{
			BufferStream buffer = Constants.Packet.CreateOutgoingBufferStream(Length);

			buffer.WriteBytes(Buffer, Index, Length);

			SendInternal(buffer);
		}

		protected abstract void ConnectInternal(IPEndPoint EndPoint);

		protected virtual void SendInternal(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer, Timestamp));
		}

		protected override void Receive()
		{
			if (!IsConnected)
				return;

			try
			{
				int size = 0;

				lock (Socket)
				{
					if (Socket.Available == 0)
					{
						if (!IsReady)
							HandleDisconnection(Socket);

						return;
					}

					size = Socket.Receive(ReceiveBuffer);
				}

				BandwidthIn += (uint)size;

				uint index = 0;
				while (index != size)
				{
					uint packetSize = BitConverter.ToUInt32(ReceiveBuffer, (int)index);

					index += Constants.Packet.PACKET_SIZE_SIZE;

					HandleIncommingBuffer(new BufferStream(ReceiveBuffer, index, packetSize));

					index += packetSize;
				}
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset)
				{
					HandleDisconnection(Socket);

					return;
				}

				throw e;
			}
		}

		protected virtual void HandleIncommingBuffer(BufferStream Buffer)
		{
			double time = Time.CurrentEpochTime;

			LastTouchTime = time;

			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = Constants.Packet.CreateIncommingBufferStream(Buffer.Buffer);

				if (MultithreadedCallbacks)
				{
					if (OnBufferReceived != null)
						CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, buffer);
				}
				else
				{
					AddEvent(new BufferReceivedvent(buffer));
				}
			}
			else if (control == Constants.Control.PING)
			{
				double sendTime = Buffer.ReadFloat64();

				Latency = (uint)((time - sendTime) * 1000);

				double t0 = lastPingTime;
				double t1 = sendTime;
				double t2 = sendTime;
				double t3 = time;

				timeOffset = ((t1 - t0) + (t2 - t3)) / 2;
			}
		}

		protected override bool HandleSendCommand(SendCommand Command)
		{
			if (Timestamp < Command.SendTime + (LatencySimulation / 1000.0F))
				return false;

			if (!SocketUtilities.GetIsReady(Socket))
				return false;

			SendInternal(Socket, Command.Buffer);

			return true;
		}

		protected override void ProcessEvent(EventBase Event)
		{
			ClientEventBase ev = (ClientEventBase)Event;

			if (ev is ConnectedEvent)
			{
				if (OnConnected != null)
					CallbackUtilities.InvokeCallback(OnConnected.Invoke);
			}
			else if (ev is ConnectionFailedEvent)
			{
				if (OnConnectionFailed != null)
					CallbackUtilities.InvokeCallback(OnConnectionFailed.Invoke);
			}
			else if (ev is BufferReceivedvent)
			{
				if (OnConnectionFailed != null)
					CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, ((BufferReceivedvent)ev).Buffer);
			}
		}

		protected override void HandleDisconnection(Socket Socket)
		{
			base.HandleDisconnection(Socket);

			if (MultithreadedCallbacks)
			{
				if (OnDisconnected != null)
					CallbackUtilities.InvokeCallback(OnDisconnected.Invoke);
			}
			else
			{
				AddEvent(new DisconnectedEvent());
			}

			IsConnected = false;
		}

		protected abstract void ProcessReceivedBuffer(BufferStream Buffer);

		protected void HandleReceivedBuffer(BufferStream Buffer)
		{
			if (MultithreadedCallbacks)
			{
				if (OnBufferReceived != null)
					CallbackUtilities.InvokeCallback(OnBufferReceived.Invoke, Buffer);
			}
			else
			{
				AddEvent(new BufferReceivedvent(Buffer));
			}
		}

		protected void RaiseOnConnectedEvent()
		{
			if (MultithreadedCallbacks)
			{
				if (OnConnected != null)
					CallbackUtilities.InvokeCallback(OnConnected.Invoke);
			}
			else
			{
				AddEvent(new ConnectedEvent());
			}
		}

		protected void RaiseOnConnectionFailedEvent()
		{
			if (MultithreadedCallbacks)
			{
				if (OnConnectionFailed != null)
					CallbackUtilities.InvokeCallback(OnConnectionFailed.Invoke);
			}
			else
			{
				AddEvent(new ConnectionFailedEvent());
			}
		}

		private void SendPing()
		{
			BufferStream pingBuffer = Constants.Packet.CreatePingBufferStream();

			lastPingTime = Time.CurrentEpochTime;

			SendInternal(Socket, pingBuffer);
		}
	}
}