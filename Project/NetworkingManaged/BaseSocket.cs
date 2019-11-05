// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Net.Sockets;

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

		public BaseSocket(Protocols Type)
		{
			events = new EventBaseList();

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

		protected abstract void Receive();

		protected abstract void ProcessEvent(EventBase Event);

		protected void AddEvent(EventBase Event)
		{
			lock (events)
				events.Add(Event);
		}
	}
}