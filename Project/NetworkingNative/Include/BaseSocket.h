// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BASE_SOCKET_H
#define BASE_SOCKET_H

#include "Common.h"
#include "NetworkingStatistics.h"
#include "SocketUtilities.h"
#include <BufferStream.h>
#include <thread>
#include <atomic>

using namespace GameFramework::BinarySerializer;

namespace GameFramework::Networking
{
	class NETWORKING_API BaseSocket
	{
	protected:
		class NETWORKING_API EventBase
		{
		public:
			EventBase(void)
			{
			}

			virtual ~EventBase(void) = default;
		};

	private:
		typedef List<EventBase*> EventBaseList;

	protected:
		class NETWORKING_API SendCommand
		{
		public:
			SendCommand(const BufferStream& Buffer, const double& SendTime) :
				m_Buffer(Buffer),
				m_SendTime(SendTime)
			{
			}

		public:
			const BufferStream& GetBuffer(void) const
			{
				return m_Buffer;
			}

			const double& GetSendTime(void) const
			{
				return m_SendTime;
			}

		private:
			BufferStream m_Buffer;
			double m_SendTime;
		};

	private:
		typedef List<SendCommand*> SendCommandList;

		friend static void GlobalReceiverWorker(BaseSocket* Instance);
		friend static void GlobalSendWorker(BaseSocket* Instance);

	public:
		BaseSocket(PlatformNetwork::IPProtocols Type);

		virtual ~BaseSocket(void);

		virtual void Service(void);

	protected:
		void Shutdown(void);

		void RunReceiveThread(void);

		void RunSenndThread(void);

		virtual void Receive(void) = 0;

		virtual bool SendOverSocket(Socket Target, const BufferStream& Buffer);

		virtual bool SendOverSocket(const IPEndPoint& EndPoint, const BufferStream& Buffer);

		virtual bool HandleSendCommand(SendCommand* Command) = 0;

		virtual void ProcessEvent(EventBase* Event) = 0;

		void AddEvent(EventBase* Event);

		void AddSendCommand(SendCommand* Command);

		virtual void HandleDisconnection(Socket Socket);

	private:
		void ReceiverWorker(void);

		void SendWorker(void);

		void HandleSendCommands(void);

	protected:
		Socket GetSocket(void) const
		{
			return m_Socket;
		}

		std::byte* GetReceiveBuffer(void)
		{
			return m_ReceiveBuffer;
		}

		uint32_t GetReceiveBufferIndex(void) const
		{
			return m_ReceiveBufferIndex;
		}
		void SetReceiveBufferIndex(uint32_t Value)
		{
			m_ReceiveBufferIndex = Value;
		}

	public:
		PlatformNetwork::IPProtocols GetType(void) const
		{
			return m_Type;
		}

		virtual bool GetIsReady(void) const = 0;

		virtual double GetTimestamp(void) const = 0;

		NetworkingStatistics& GetStatistics(void)
		{
			return m_Statistics;
		}

		const NetworkingStatistics& GetStatistics(void) const
		{
			return m_Statistics;
		}

		uint32_t GetSendBufferSize(void) const
		{
			return m_SendBufferSize;
		}
		void SetSendBufferSize(uint32_t Value)
		{
			m_SendBufferSize = Value;
		}

		uint32_t GetReceiveBufferSize(void) const
		{
			return m_ReceiveBufferSize;
		}
		void SetReceiveBufferSize(uint32_t Value)
		{
			m_ReceiveBufferSize = Value;
		}

		bool GetMultithreadedCallbacks(void) const
		{
			return m_MultithreadedCallbacks;
		}
		void SetMultithreadedCallbacks(bool Value)
		{
			m_MultithreadedCallbacks = Value;
		}

		bool GetMultithreadedReceive(void) const
		{
			return m_MultithreadedReceive;
		}
		void SetMultithreadedReceive(bool Value)
		{
			m_MultithreadedReceive = Value;
		}

		bool GetMultithreadedSend(void) const
		{
			return m_MultithreadedSend;
		}
		void SetMultithreadedSend(bool Value)
		{
			m_MultithreadedSend = Value;
		}

		float GetPacketLossSimulation(void) const
		{
			return m_PacketLossSimulation;
		}
		void SetPacketLossSimulation(float Value)
		{
			m_PacketLossSimulation = Value;
		}

		int GetLatencySimulation(void) const
		{
			return m_LatencySimulation;
		}
		void SetLatencySimulation(int Value)
		{
			m_LatencySimulation = Value;
		}

	private:
		thread* m_ReceiveThread;
		thread* m_SendThread;

		EventBaseList m_Events;
		atomic_bool m_EventsLock;

		SendCommandList m_SendCommands;
		atomic_bool m_SendCommandsLock;

		PlatformNetwork::IPProtocols m_Type;
		Socket m_Socket;

		std::byte* m_ReceiveBuffer;
		uint32_t m_ReceiveBufferIndex;

		NetworkingStatistics m_Statistics;

		uint32_t m_SendBufferSize;
		uint32_t m_ReceiveBufferSize;

		bool m_MultithreadedCallbacks;
		bool m_MultithreadedReceive;
		bool m_MultithreadedSend;

		float m_PacketLossSimulation;
		int m_LatencySimulation;
	};
}

#endif