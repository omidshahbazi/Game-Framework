// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef NETWORKING_STATISTICS_H
#define NETWORKING_STATISTICS_H

#include "Common.h"

namespace GameFramework::Networking
{
	class NETWORKING_API NetworkingStatistics
	{
	public:
		NetworkingStatistics(void);

		double GetLastTouchTime(void) const
		{
			return m_LastTouchTime;
		}

		void SetLastTouchTime(double Time)
		{
			m_LastTouchTime = Time;
		}

		uint32_t GetLatency(void) const
		{
			return m_Latency;
		}

		void SetLatency(uint32_t Latency)
		{
			m_Latency = Latency;
		}

		uint32_t GetReceivedPacketFromLastSecond(void) const
		{
			return m_ReceivedPacketFromLastSecond;
		}

		void AddReceivedPacketFromLastSecond(void)
		{
			++m_ReceivedPacketFromLastSecond;
		}

		void ResetReceivedPacketFromLastSecond(void)
		{
			m_ReceivedPacketFromLastSecond = 0;
		}

		uint64_t GetBandwidthIn(void) const
		{
			return m_BandwidthIn;
		}

		void AddBandwidthIn(uint32_t Size)
		{
			m_BandwidthIn += Size;
		}

		uint64_t GetBandwidthOut(void) const
		{
			return m_BandwidthOut;
		}

		void AddBandwidthOut(uint32_t Size)
		{
			m_BandwidthOut += Size;
		}

		uint32_t GetPacketCountRate(void) const
		{
			return m_PacketCountRate;
		}

		void SetPacketCountRate(uint32_t PacketCountRate)
		{
			m_PacketCountRate = PacketCountRate;
		}

	private:
		double m_LastTouchTime;
		uint32_t m_Latency;
		uint32_t m_ReceivedPacketFromLastSecond;
		uint64_t m_BandwidthIn;
		uint64_t m_BandwidthOut;
		uint32_t m_PacketCountRate;
	};
}

#endif