// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_CLIENT_SOCKET_H
#define TCP_CLIENT_SOCKET_H

#include "ClientSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API TCPClientSocket : public ClientSocket
	{
	public:
		TCPClientSocket(void);

		virtual void Send(byte* const Buffer, uint32_t Length);

		virtual void Send(byte* const Buffer, uint32_t Index, uint32_t Length);

		virtual void Service(void) override;

		virtual void Disconnect(void) override;

	protected:
		virtual void SendInternal(const BufferStream& Buffer);

		virtual void ConnectInternal(const IPEndPoint& EndPoint) override;

		virtual void Receive(void) override;

		virtual void HandleIncomingBuffer(BufferStream& Buffer) override;

		virtual void ProcessReceivedBuffer(const BufferStream& Buffer) override;

		virtual BufferStream GetPingPacket(void) override;

	private:
		void CheckConnectionStatus(void);

	private:
		bool m_IsConnecting;
	};
}

#endif