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
		Client(Socket Socket, const IPEndPoint &EndPoint);

		void UpdateLastTouchTime(double Time)
		{
			m_LastTouchTime = Time;
		}

		void UpdateLatency(uint32_t Latency)
		{
			m_Latency = Latency;
		}

		bool IsReady(void) const;

		Socket GetSocket(void) const
		{
			return m_Socket;
		}

		const IPEndPoint &GetEndPoint(void) const
		{
			return m_EndPoint;
		}

		double GetLastTouchTime(void) const
		{
			return m_LastTouchTime;
		}

		uint32_t GetLatency(void) const
		{
			return m_Latency;
		}

	private:
		Socket m_Socket;
		IPEndPoint m_EndPoint;
		double m_LastTouchTime;
		uint32_t m_Latency;
	};

	typedef List<Client*> ClientList;
}

#endif