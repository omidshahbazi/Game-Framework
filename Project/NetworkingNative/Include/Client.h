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
		Client(Socket Socket) :
			m_Socket(Socket)
		{
		}

		Socket GetSocket(void) const
		{
			return m_Socket;
		}

	private:
		Socket m_Socket;
	};

	typedef List<Client*> ClientList;
}

#endif