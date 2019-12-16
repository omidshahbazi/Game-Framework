// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;

namespace GameFramework.Networking
{
	public class NetworkingStatistics
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

		public uint ReceivedPacketFromLastSecond
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

		public NetworkingStatistics()
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

		public void AddReceivedPacketFromLastSecond()
		{
			++ReceivedPacketFromLastSecond;
		}

		public void ResetReceivedPacketFromLastSecond()
		{
			ReceivedPacketFromLastSecond = 0;
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
}