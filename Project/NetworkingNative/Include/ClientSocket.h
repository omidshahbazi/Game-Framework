// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CLIENT_SOCKET_H
#define CLIENT_SOCKET_H

#include "BaseSocket.h"
#include <Utilities\Event.h>

using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	class NETWORKING_API ClientSocket : public BaseSocket
	{
	private:
		class ClientEventBase : public EventBase
		{
		public:
			ClientEventBase(void)
			{
			}

			virtual ~ClientEventBase(void) = default;
		};

		class ConnectedEvent : public ClientEventBase
		{
		public:
			ConnectedEvent(void) : ClientEventBase()
			{ }
		};

		class ConnectionFailedEvent : public ClientEventBase
		{
		public:
			ConnectionFailedEvent(void) : ClientEventBase()
			{ }
		};

		class DisconnectedEvent : public ClientEventBase
		{
		public:
			DisconnectedEvent(void) : ClientEventBase()
			{ }
		};

		class BufferReceivedvent : public ClientEventBase
		{
		public:
			BufferReceivedvent(BufferStream Buffer) :
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

	public:
		ClientSocket(PlatformNetwork::IPProtocols Type);

		virtual void Service(void) override;

		void Connect(const std::string& Host, uint16_t Port);

		void Connect(const IPAddress& IP, uint16_t Port);

		void Connect(const IPEndPoint& EndPoint);

		void Disconnect(void);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Length);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length);

	protected:
		virtual void SendInternal(const BufferStream& Buffer);

		virtual void Receive(void) override;

		virtual void HandleIncommingBuffer(BufferStream& Buffer);

		virtual bool HandleSendCommand(SendCommand* Command) override;

		virtual void ProcessEvent(EventBase* Event)  override;

		virtual void HandleDisconnection(Socket Socket) override;

		virtual	void ProcessReceivedBuffer(const BufferStream& Buffer) = 0;

		void HandleReceivedBuffer(const BufferStream& Buffer);

		virtual bool GetIsReady(void) override
		{
			return m_IsBound;
		}

		virtual double GetTimestamp(void)  override;

	private:
		void SendPing(void);

	public:
		SimpleEvent OnConnected;
		SimpleEvent OnConnectionFailed;
		SimpleEvent OnDisconnected;
		Event<BufferStream> OnBufferReceived;

	private:
		bool m_IsBound;

		ClientList m_Clients;
		atomic_bool m_ClientsLock;

		uint32_t m_MaxConnection;
	};
}

#endif