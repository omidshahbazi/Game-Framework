// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_CLIENT_SOCKET_H
#define TCP_CLIENT_SOCKET_H

#include "ClientSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API UDPClientSocket : public ClientSocket
	{
	public:
		UDPClientSocket(void);

	protected:
		void ConnectInternal(const IPEndPoint& EndPoint) override;

		void ProcessReceivedBuffer(const BufferStream &Buffer) override;
	};
}

#endif