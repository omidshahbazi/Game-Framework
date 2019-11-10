// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public class Client
	{
		public bool IsConnected
		{
			get { return Socket.Connected; }
		}

		public Socket Socket
		{
			get;
			private set;
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

		public Client(Socket Socket)
		{
			this.Socket = Socket;

			LastTouchTime = Time.CurrentEpochTime;
		}

		public void UpdateLastTouchTime(double Time)
		{
			LastTouchTime = Time;
		}

		public void UpdateLatency(uint Latency)
		{
			this.Latency = Latency;
		}
	}

	public class ClientList : List<Client>
	{ }
}