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

		public const float PING_TIME = 5;

		private double nextPingTime;
		private BufferStream pingBuffer;

		public event ConnectionEventHandler OnConnected = null;
		public event ConnectionEventHandler OnConnectionFailed = null;
		public event ConnectionEventHandler OnDisconnected = null;
		public event BufferReceivedEventHandler OnBufferReceived = null;

		public ClientSocket(Protocols Type) : base(Type)
		{
			pingBuffer = new BufferStream(new byte[10]);
		}

		public override void Service()
		{
			if (Time.CurrentEpochTime >= nextPingTime)
			{
				nextPingTime = Time.CurrentEpochTime + PING_TIME;

				UpdatePingBuffer();

				Send(Socket, pingBuffer);
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
			Send(Buffer, 0, Buffer.Length);
		}

		public virtual void Send(byte[] Buffer, int Length)
		{
			Send(Buffer, 0, Length);
		}

		public virtual void Send(byte[] Buffer, int Index, int Length)
		{
			BufferStream buffer = new BufferStream(new byte[Constants.Packet.HEADER_SIZE + Length]);
			buffer.Reset();
			buffer.WriteBytes(Constants.Control.BUFFER);
			buffer.WriteBytes(Buffer, Index, Length);

			Send(buffer);
		}

		protected virtual void Send(BufferStream Buffer)
		{
			AddSendCommand(new SendCommand(Buffer));
		}

		protected override void Receive()
		{
			if (!Socket.Connected)
				return;

			try
			{
				if (Socket.Available == 0)
					return;

				int size = Socket.Receive(ReceiveBuffer);

				BandwidthIn += (uint)size;

				HandleIncommingBuffer(new BufferStream(ReceiveBuffer, (uint)size));
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset)
				{
					HandleDisconnection();

					return;
				}

				throw e;
			}
		}

		protected virtual void HandleIncommingBuffer(BufferStream Buffer)
		{
			byte control = Buffer.ReadByte();

			if (control == Constants.Control.BUFFER)
			{
				BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.Packet.HEADER_SIZE, Buffer.Size - Constants.Packet.HEADER_SIZE);

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

			}
		}

		protected override void HandleSendCommand(SendCommand Command)
		{
			Send(Socket, Command.Buffer);
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

			HandleDisconnection();
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
				Socket.EndConnect(Result);

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

		private void HandleDisconnection()
		{
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

		private void UpdatePingBuffer()
		{
			//Add IOControl byte
			//buffer
			//ping
			//fill rtt
			pingBuffer.Reset();
			pingBuffer.WriteBytes(Constants.Control.PING);
			pingBuffer.WriteFloat64(Time.CurrentEpochTime);
		}
	}
}