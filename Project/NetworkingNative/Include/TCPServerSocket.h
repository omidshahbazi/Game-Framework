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

		typedef List<TCPClient*> TCPClientList;

	public:
		TCPServerSocket(uint32_t MaxConnection);

		void UnBind(void) override;

		void DisconnectClient(Client* const Client) override;

		virtual void Listen(void) override;

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Length);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length);

	protected:
		virtual void AddSendCommand(Client* Target, const BufferStream& Buffer);

		virtual void AcceptClients(void);

		virtual void ReadFromClients(void);

		void HandleIncomingBuffer(Client* Client, BufferStream& Buffer);

		bool HandleSendCommand(SendCommand* Command) override;

		void ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer) override;

		void CloseClientConnection(Client* Client) override;

	public:
		uint32_t GetMaxConnection(void) const
		{
			return m_MaxConnection;
		}

	private:
		uint32_t m_MaxConnection;

		TCPClientList m_Clients;
		atomic_bool m_ClientsLock;
	};
}

#endif