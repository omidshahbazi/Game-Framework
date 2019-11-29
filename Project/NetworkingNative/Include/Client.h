// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CLIENT_H
#define CLIENT_H

#include "Common.h"
#include "SocketUtilities.h"

namespace GameFramework::Networking
{
	class NETWORKING_API Client
	{
	public:

		Socket GetSocket(void) const
		{
			return 0;
		}
	};

	typedef List<Client*> ClientList;
}

#endif