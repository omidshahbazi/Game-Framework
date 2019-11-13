// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;
using System.Collections.Generic;
using System.Net.Sockets;

namespace GameFramework.Networking
{
	public class Client
	{
		public bool IsReady
		{
			get { return SocketUtilities.IsSocketReady(Socket); }//&& (Time.CurrentEpochTime - LastTouchTime < Constants.PING_TIME * 2)
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