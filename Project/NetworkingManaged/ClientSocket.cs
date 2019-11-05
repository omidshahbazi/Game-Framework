// Copyright 2019. All Rights Reserved.
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

		public delegate void ConnectionEventHandler();

		public event ConnectionEventHandler OnConnected = null;
		public event ConnectionEventHandler OnConnectionFailed = null;

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

		public void Send()
		{
			Socket.Send(new byte[] { 2, 3, 4 });
		}

		protected override void Receive()
		{
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
		}

		private void OnConnectedCallback(IAsyncResult Result)
		{
			if (Socket.Connected)
			{
				Socket.EndConnect(Result);

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