// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_SERVER_SOCKET_H
#define TCP_SERVER_SOCKET_H

#include "ServerSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API TCPServerSocket : public ServerSocket
	{
	public:
		TCPServerSocket(uint32_t MaxConnection);

	protected:
		void ProcessReceivedBuffer(Client *Sender, const BufferStream &Buffer) override;
	};
}

#endif