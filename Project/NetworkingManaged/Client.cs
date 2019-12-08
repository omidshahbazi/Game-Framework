// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;
using System.Collections.Generic;
using System.Net;

namespace GameFramework.Networking
{
	public abstract class Client
	{
		public virtual bool IsReady
		{
			get { return (Time.CurrentEpochTime - LastTouchTime < Constants.PING_TIME * 2); }
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

		public abstract IPEndPoint EndPoint
		{
			get;
		}

		public uint BandwidthInFromLastSecond
		{
			get;
			private set;
		}

		public Client()
		{
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

		public void AddBandwidthInFromLastSecond(uint Count)
		{
			BandwidthInFromLastSecond += Count;
		}

		public void ResetBandwidthInFromLastSecond()
		{
			BandwidthInFromLastSecond = 0;
		}
	}

	public class ClientList : List<Client>
	{ }
}