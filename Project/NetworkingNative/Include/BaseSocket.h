﻿// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BASE_SOCKET_H
#define BASE_SOCKET_H

#include "Common.h"
#include "PlatformNetwork.h"
#include <BufferStream.h>
#include <thread>

using namespace GameFramework::BinarySerializer;
using namespace std;

namespace GameFramework::Networking
{
	class NETWORKING_API BaseSocket
	{
	protected:
		class EventBase
		{
		public:
			EventBase()
			{
			}
		};

	private:
		typedef List<EventBase> EventBaseList;

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

		typedef PlatformNetwork::Handle Socket;

	private:
		typedef List<SendCommand> SendCommandList;

	public:
		BaseSocket(PlatformNetwork::IPProtocols Type);

	protected:
		void Shutdown(void);

		void RunReceiveThread(void);

		void RunSenndThread(void);

		virtual void Receive(void) = 0;

		virtual void Send(Socket Target, BufferStream Buffer);

		virtual bool HandleSendCommand(SendCommand Command) = 0;

		virtual void ProcessEvent(EventBase Event) = 0;

		void AddEvent(EventBase Event);

		void AddSendCommand(SendCommand Command);

		virtual void HandleDisconnection(Socket Socket);

	private:
		void ReceiverWorker(void);

		void SendWorker(void);

		void HandleSendCommands(void);

	protected:
		Socket& GetSocket(void)
		{
			return m_Socket;
		}

		byte* GetReceiveBuffer(void)
		{
			return m_ReceiveBuffer;
		}

		virtual bool GetIsReady(void) = 0;

		virtual double GetTimestamp(void) = 0;

	public:
		const uint64_t& GetBandwidthIn(void) const
		{
			return m_BandwidthIn;
		}

		const uint64_t& GetBBandwidthOut(void) const
		{
			return m_BandwidthOut;
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

		bool GetPacketLossSimulation(void) const
		{
			return m_PacketLossSimulation;
		}
		void SetPacketLossSimulation(bool Value)
		{
			m_PacketLossSimulation = Value;
		}

		bool GetLatencySimulation(void) const
		{
			return m_LatencySimulation;
		}
		void SetLatencySimulation(bool Value)
		{
			m_LatencySimulation = Value;
		}

	private:
		thread m_ReceiveThread;
		thread m_SendThread;

		EventBaseList m_Events;
		SendCommandList m_SendCommands;

	private:
		Socket m_Socket;

		byte* m_ReceiveBuffer;

		uint64_t m_BandwidthIn;
		uint64_t m_BandwidthOut;

		bool m_MultithreadedCallbacks;
		bool m_MultithreadedReceive;
		bool m_MultithreadedSend;

		float m_PacketLossSimulation;
		int m_LatencySimulation;
	};
}

#endif