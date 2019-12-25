// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef SERVER_SOCKET_H
#define SERVER_SOCKET_H

#include "BaseSocket.h"
#include "Client.h"
#include <Utilities\Event.h>

using namespace GameFramework::Common::Utilities;

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
			ServerSendCommand(Client* Client, const BufferStream& Buffer, double SendTime) : SendCommand(Buffer, SendTime),
				m_Client(Client)
			{
			}

			Client* GetClient(void)
			{
				return m_Client;
			}

		private:
			Client* m_Client;
		};

	public:
		ServerSocket(PlatformNetwork::IPProtocols Type);

		void Bind(const std::string& Host, uint16_t Port);

		void Bind(const IPAddress& IP, uint16_t Port);

		void Bind(const IPEndPoint& EndPoint);

		virtual void UnBind(void);

		virtual void DisconnectClient(Client* const Client);

		void virtual Listen(void);

	protected:
		virtual void Receive(void) override;

		virtual void AcceptClients(void) = 0;

		virtual void ReadFromClients(void) = 0;

		virtual void ProcessReceivedBuffer(Client* Client, uint32_t Size);

		virtual void HandleIncomingBuffer(Client* Client, BufferStream& Buffer) = 0;

		virtual void ProcessEvent(EventBase* Event)  override;

		virtual	void ProcessReceivedBuffer(Client* Sender, BufferStream& Buffer) = 0;

		void HandleReceivedBuffer(Client* Sender, const BufferStream& Buffer);

		virtual void CloseClientConnection(Client* Client)
		{
		}

		void HandleClientDisconnection(Client* Client);

		void RaiseOnClientConnected(Client* Client);

	public:
		const IPEndPoint& GetLocalEndPoint(void) const
		{
			return m_LocalEndPoint;
		}

		virtual bool GetIsReady(void) const override
		{
			return m_IsBound;
		}

		virtual double GetTimestamp(void) const override;

		virtual const Client* GetClients(void) const = 0;
		virtual uint32_t GetClientCount(void) const = 0;

		uint32_t GetPacketCountRate(void) const
		{
			return m_PacketCountRate;
		}

	public:
		Event<const Client*> OnClientConnected;
		Event<const Client*> OnClientDisconnected;
		Event<const Client*, BufferStream> OnBufferReceived;

	private:
		IPEndPoint m_LocalEndPoint;
		bool m_IsBound;
		uint32_t m_PacketCountRate;
	};
}

#endif