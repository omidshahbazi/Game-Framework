// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef SERVER_SOCKET_H
#define SERVER_SOCKET_H

#include "BaseSocket.h"
#include "Client.h"

namespace GameFramework::Networking
{
	class NETWORKING_API ServerSocket : public BaseSocket
	{
	private:
		class ServerEventBase : public EventBase
		{
		public:
			ServerEventBase(Client* Client) :
				m_Client(Client)
			{
			}

			virtual ~ServerEventBase(void) = default;

			const Client* GetClient(void) const
			{
				return m_Client;
			}

		private:
			Client* m_Client;
		};

		class ClientConnectedEvent : public ServerEventBase
		{
		public:
			ClientConnectedEvent(Client* Client) : ServerEventBase(Client)
			{ }
		};

		class ClientDisconnectedEvent : public ServerEventBase
		{
		public:
			ClientDisconnectedEvent(Client* Client) : ServerEventBase(Client)
			{ }
		};

		class BufferReceivedvent : public ServerEventBase
		{
		public:
			BufferReceivedvent(Client* Client, BufferStream Buffer) : ServerEventBase(Client),
				m_Buffer(Buffer)
			{
			}

			const BufferStream& GetBuffer(void) const
			{
				return m_Buffer;
			}

		private:
			BufferStream m_Buffer;
		};

	protected:
		class ServerSendCommand : public SendCommand
		{
		public:
			ServerSendCommand(Socket Socket, const BufferStream& Buffer, double SendTime) : SendCommand(Buffer, SendTime),
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


	public:
		ServerSocket(PlatformNetwork::IPProtocols Type, uint32_t MaxConnection);

		void Bind(const std::string& Host, uint16_t Port);

		void Bind(const IPAddress& IP, uint16_t Port);

		void Bind(const IPEndPoint& EndPoint);

		void UnBind(void);

		void DisconnectClient(Client* const Client);

		void Listen(void);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Length);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length);

	protected:
		virtual void Send(const Client* Target, const BufferStream& Buffer);

		virtual void Receive(void) override;

		virtual void HandleIncommingBuffer(Client* Client, BufferStream& Buffer);

		virtual bool HandleSendCommand(SendCommand* Command) override;

		virtual void ProcessEvent(EventBase* Event)  override;

		virtual	void ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer) = 0;

		void HandleReceivedBuffer(Client* Sender, const BufferStream& Buffer);

		virtual bool GetIsReady(void) override
		{
			return m_IsBound;
		}

		virtual double GetTimestamp(void)  override;

	private:
		void HandleClientDisconnection(Client* Client);

	private:
		bool m_IsBound;

		ClientList m_Clients;
		atomic_bool m_ClientsLock;

		uint32_t m_MaxConnection;
	};
}

#endif