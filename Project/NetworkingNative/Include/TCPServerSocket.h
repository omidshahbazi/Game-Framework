// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_SERVER_SOCKET_H
#define TCP_SERVER_SOCKET_H

#include "ServerSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API TCPServerSocket : public ServerSocket
	{
	private:
		class TCPClient : public Client
		{
		public:
			TCPClient(Socket Socket, const IPEndPoint& EndPoint) :
				m_Socket(Socket),
				m_EndPoint(EndPoint)
			{

			}

			bool GetIsReady(void) const override
			{
				return (SocketUtilities::GetIsReady(m_Socket) && Client::GetIsReady());
			}

			Socket GetSocket(void) const
			{
				return m_Socket;
			}

			const IPEndPoint& GetEndPoint(void) const
			{
				return m_EndPoint;
			}

		private:
			Socket m_Socket;
			IPEndPoint m_EndPoint;
		};

	public:
		TCPServerSocket(uint32_t MaxConnection);

		virtual void Listen(void) override;

	protected:
		void ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer) override;

	public:
		uint32_t GetMaxConnection(void) const
		{
			return m_MaxConnection;
		}

	private:
		uint32_t m_MaxConnection;
	};
}

#endif