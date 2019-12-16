// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CLIENT_H
#define CLIENT_H

#include "NetworkingStatistics.h"
#include "SocketUtilities.h"

namespace GameFramework::Networking
{
	class NETWORKING_API Client
	{
	public:
		Client(void)
		{
		}

		virtual bool GetIsReady(void) const;

		NetworkingStatistics& GetStatistics(void)
		{
			return m_Statistics;
		}

		const NetworkingStatistics& GetStatistics(void) const
		{
			return m_Statistics;
		}

		virtual const IPEndPoint& GetEndPoint(void) const = 0;

	private:
		NetworkingStatistics m_Statistics;
		uint32_t m_Latency;
	};

	typedef List<Client*> ClientList;
}

#endif