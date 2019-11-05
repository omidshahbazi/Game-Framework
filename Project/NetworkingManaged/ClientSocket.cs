// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
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

		public event ConnectionEventHandler OnConnected = null;
		public event ConnectionEventHandler OnConnectionFailed = null;
		public event ConnectionEventHandler OnDisconnected = null;
		public event BufferReceivedEventHandler OnBufferReceived = null;

		public ClientSocket(Protocols Type) : base(Type)
		{
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

		public void Send()
		{
			Socket.Send(new byte[] { 2, 3, 4 });
		}

		protected override void Receive()
		{
			if (!Socket.Connected)
				return;

			try
			{
				int size = Socket.Receive(ReceiveBuffer);

				BandwidthIn += (uint)size;
			}
			catch (SocketException e)
			{
				if (e.SocketErrorCode == SocketError.ConnectionReset)
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

					return;
				}

				throw e;
			}
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

				if (MultithreadedReceive)
					RunReceiveThread();

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
	}
}