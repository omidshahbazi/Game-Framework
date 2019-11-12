// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Common.Timing;
using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
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

		public override bool IsReady
		{
			get { return SocketUtilities.IsSocketReady(Socket); }
		}

		public double ServerTime
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
			if (LastTouchTime + Constants.PING_TIME <= Time.CurrentEpochTime)
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

			Socket.BeginConnect(EndPoint, OnConnectedCallback, null);
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

			Send(buffer);
		}

		protected virtual void Send(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer));
		}

		protected override void Receive()
		{
			try
			{
				int size = 0;

				lock (Socket)
				{
					if (Socket.Available == 0)
						return;

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
			if (!SocketUtilities.IsSocketReady(Socket))
				return false;

			Send(Socket, Command.Buffer);

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

		private void OnConnectedCallback(IAsyncResult Result)
		{
			if (Socket.Connected)
			{
				lock (Socket)
				{
					Socket.EndConnect(Result);
				}

				RunReceiveThread();
				RunSenndThread();

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
			else
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
		}

		private void SendPing()
		{
			BufferStream pingBuffer = Constants.Packet.CreatePingBufferStream();

			lastPingTime = Time.CurrentEpochTime;

			Send(pingBuffer);
		}
	}
}