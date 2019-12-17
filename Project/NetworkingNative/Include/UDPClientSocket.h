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
		virtual void SendInternal(BufferStream &Buffer);

		void ConnectInternal(const IPEndPoint& EndPoint) override;

		void ProcessReceivedBuffer(const BufferStream& Buffer) override;

		virtual void HandleIncomingBuffer(BufferStream& Buffer) override
		{

		}

		virtual BufferStream GetPingPacket(void) override
		{
			return BufferStream(0);
		}

		uint32_t GetMTU(void) const
		{
			return m_MTU;
		}

	private:
		uint32_t m_MTU;
	};
}

#endif