// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace GameFramework.NetworkingManaged
{
	public abstract class BaseSocket
	{
		protected abstract class EventBase
		{
			public EventBase()
			{
			}
		}

		private class EventBaseList : List<EventBase>
		{ }

		private EventBaseList events = null;

		protected Thread ReceiveThread
		{
			get;
			private set;
		}

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

		public ulong BandwidthIn
		{
			get;
			protected set;
		}

		public ulong BandwidthOut
		{
			get;
			protected set;
		}

		public bool MultithreadedCallbacks
		{
			get;
			set;
		}

		public bool MultithreadedReceive
		{
			get;
			set;
		}

		public bool MultithreadedSend
		{
			get;
			set;
		}

		public float PacketLossSimulation // TODO: use this
		{
			get;
			set;
		}

		public int LatencySimulation // TODO: use this
		{
			get;
			set;
		}

		public const uint RECEIVE_BUFFER_SIZE = 1024;
		public const uint SEND_BUFFER_SIZE = 1024;

		public BaseSocket(Protocols Type)
		{
			events = new EventBaseList();

			Socket = SocketUtilities.CreateSocket(Type);
			Socket.Blocking = false;
			Socket.ReceiveBufferSize = (int)RECEIVE_BUFFER_SIZE;
			Socket.SendBufferSize = (int)SEND_BUFFER_SIZE;

			SocketUtilities.SetIPv6OnlyEnabled(Socket, false);
			SocketUtilities.SetChecksumEnabled(Socket, false);
			SocketUtilities.SetDelayEnabled(Socket, false);

			ReceiveBuffer = new byte[RECEIVE_BUFFER_SIZE];

			MultithreadedCallbacks = true;
			MultithreadedReceive = true;
			MultithreadedSend = true;
		}

		public void Service()
		{
			if (!MultithreadedReceive)
				Receive();

			if (!MultithreadedCallbacks)
			{
				lock (events)
				{
					for (int i = 0; i < events.Count; ++i)
						ProcessEvent(events[i]);

					events.Clear();
				}
			}
		}

		protected void Shutdown()
		{
			SocketUtilities.CloseSocket(Socket);

			if (MultithreadedReceive)
				ReceiveThread.Abort();
		}

		protected void RunReceiveThread()
		{
			ReceiveThread = new Thread(ReceiverWorker);
			ReceiveThread.Start();
		}

		protected abstract void Receive();

		protected abstract void ProcessEvent(EventBase Event);

		protected void AddEvent(EventBase Event)
		{
			lock (events)
				events.Add(Event);
		}

		private void ReceiverWorker()
		{
			while (true)
			{
				Thread.Sleep(1);

				Receive();
			}
		}
	}
}