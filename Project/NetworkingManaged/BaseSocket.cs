// Copyright 2019. All Rights Reserved.
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public abstract class BaseSocket
	{
		protected Socket Socket
		{
			get;
			private set;
		}

		protected byte[] ReceiveBuffer
		{
			get;
			private set;
		}

		public bool MultithreadedCallbacks
		{
			get;
			private set;
		}

		public bool MultithreadedReceive
		{
			get;
			private set;
		}

		public bool MultithreadedSend
		{
			get;
			private set;
		}

		public BaseSocket(Protocols Type)
		{
			Socket = SocketUtilities.CreateSocket(Type);
			Socket.Blocking = false;

			SocketUtilities.SetIPv6OnlyEnabled(Socket, false);
			SocketUtilities.SetChecksumEnabled(Socket, false);
			SocketUtilities.SetDelayEnabled(Socket, false);

			ReceiveBuffer = new byte[1024];

			MultithreadedCallbacks = true;
			MultithreadedReceive = true;
			MultithreadedSend = true;
		}

		public void Service()
		{
			if (!MultithreadedReceive)
				Receive();
		}

		protected abstract void Receive();
	}
}