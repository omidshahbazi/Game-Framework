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

		virtual void Connect(const IPEndPoint& EndPoint);

		virtual void Disconnect(void);

		virtual void Send(byte* const Buffer, uint32_t Length);

		virtual void Send(byte* const Buffer, uint32_t Index, uint32_t Length);

	protected:
		virtual void ConnectInternal(const IPEndPoint& EndPoint) = 0;

		virtual void SendInternal(const BufferStream& Buffer);

		virtual void Receive(void) override;

		virtual void HandleIncommingBuffer(BufferStream& Buffer);

		virtual bool HandleSendCommand(SendCommand* Command) override;

		virtual void ProcessEvent(EventBase* Event)  override;

		virtual void HandleDisconnection(Socket Socket) override;

		virtual	void ProcessReceivedBuffer(const BufferStream& Buffer) = 0;

		void HandleReceivedBuffer(const BufferStream& Buffer);

	public:
		bool GetIsConnected(void) const
		{
			return m_IsConnected;
		}

	protected:
		void SetIsConnected(bool Value)
		{
			m_IsConnected = Value;
		}

	public:
		double GetLastTouchTime(void) const
		{
			return m_LastTouchTime;
		}

		uint32_t GetLatency(void) const
		{
			return m_Latency;
		}

		virtual bool GetIsReady(void) const override
		{
			return SocketUtilities::GetIsReady(GetSocket());
		}

		virtual double GetTimestamp(void) const override;

	protected:
		void RaiseOnConnectedEvent(void);

		void RaiseOnConnectionFailedEvent(void);

	private:
		void SendPing(void);

	public:
		SimpleEvent OnConnected;
		SimpleEvent OnConnectionFailed;
		SimpleEvent OnDisconnected;
		Event<BufferStream> OnBufferReceived;

	private:
		bool m_IsConnected;
		double m_LastPingTime;
		double m_TimeOffset;
		double m_LastTouchTime;
		uint32_t m_Latency;
	};
}

#endif