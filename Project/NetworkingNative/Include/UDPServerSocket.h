// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef UDP_SERVER_SOCKET_H
#define UDP_SERVER_SOCKET_H

#include "ServerSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API UDPServerSocket : public ServerSocket
	{
	public:
		UDPServerSocket(uint32_t MaxConnection);

	protected:
		void ProcessReceivedBuffer(Client *Sender, const BufferStream &Buffer) override;
	};
}

#endif