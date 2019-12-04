// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CLIENT_H
#define CLIENT_H

#include "SocketUtilities.h"

namespace GameFramework::Networking
{
	class NETWORKING_API Client
	{
	public:
		Client(void);

		void UpdateLastTouchTime(double Time)
		{
			m_LastTouchTime = Time;
		}

		void UpdateLatency(uint32_t Latency)
		{
			m_Latency = Latency;
		}

		virtual bool GetIsReady(void) const;

		double GetLastTouchTime(void) const
		{
			return m_LastTouchTime;
		}

		uint32_t GetLatency(void) const
		{
			return m_Latency;
		}

	private:
		double m_LastTouchTime;
		uint32_t m_Latency;
	};

	typedef List<Client*> ClientList;
}

#endif