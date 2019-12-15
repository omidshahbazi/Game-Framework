// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;
using System;
using System.Collections.Generic;
using System.Net;

namespace GameFramework.Networking
{
	public class NetowrkingStatistics
	{
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

		public uint RecievedPacketFromLastSecond
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

		public NetowrkingStatistics()
		{
			LastTouchTime = Time.CurrentEpochTime;
		}

		public void SetLastTouchTime(double Time)
		{
			LastTouchTime = Time;
		}

		public void SetLatency(uint Latency)
		{
			this.Latency = Latency;
		}

		public void AddRecievedPacketFromLastSecond()
		{
			++RecievedPacketFromLastSecond;
		}

		public void ResetRecievedPacketFromLastSecond()
		{
			RecievedPacketFromLastSecond = 0;
		}

		public void AddBandwidthIn(uint Size)
		{
			BandwidthIn += Size;
		}

		public void AddBandwidthOut(uint Size)
		{
			BandwidthOut += Size;
		}
	}

	public abstract class Client
	{
		public virtual bool IsReady
		{
			//get { return (Time.CurrentEpochTime - Statistics.LastTouchTime < Constants.PING_TIME * 2); }
			get { return true; }
		}

		public NetowrkingStatistics Statistics
		{
			get;
			private set;
		}

		public abstract IPEndPoint EndPoint
		{
			get;
		}

		public Client()
		{
			Statistics = new NetowrkingStatistics();
		}
	}

	public class ClientList : List<Client>
	{ }
}