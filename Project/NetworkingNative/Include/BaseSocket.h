// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BASE_SOCKET_H
#define BASE_SOCKET_H

#include "Common.h"
#include <BufferStream.h>

using namespace GameFramework::BinarySerializer;

namespace GameFramework::NetworkingManaged
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

	private:
		typedef List<SendCommand> SendCommandList;

		//public BaseSocket(Protocols Type)
		//{
		//	events = new EventBaseList();
		//	sendCommands = new SendCommandList();

		//	Socket = SocketUtilities.CreateSocket(Type);
		//	Socket.Blocking = false;
		//	Socket.ReceiveBufferSize = (int)Constants.RECEIVE_BUFFER_SIZE;
		//	Socket.SendBufferSize = (int)Constants.SEND_BUFFER_SIZE;
		//	Socket.ReceiveTimeout = (int)Constants.RECEIVE_TIMEOUT;
		//	Socket.SendTimeout = (int)Constants.SEND_TIMEOUT;
		//	Socket.Ttl = Constants.TIME_TO_LIVE;

		//	SocketUtilities.SetIPv6OnlyEnabled(Socket, false);
		//	SocketUtilities.SetChecksumEnabled(Socket, false);
		//	SocketUtilities.SetNagleAlgorithmEnabled(Socket, false);
		//	SocketUtilities.SetBSDUrgentEnabled(Socket, true);

		//	ReceiveBuffer = new byte[Constants.RECEIVE_BUFFER_SIZE];

		//	MultithreadedCallbacks = true;
		//	MultithreadedReceive = true;
		//	MultithreadedSend = true;
		//}

		//public virtual void Service()
		//{
		//	if (!MultithreadedReceive && IsReady)
		//		Receive();

		//	if (!MultithreadedSend && IsReady)
		//		HandleSendCommands();

		//	if (!MultithreadedCallbacks)
		//	{
		//		lock(events)
		//		{
		//			for (int i = 0; i < events.Count; ++i)
		//				ProcessEvent(events[i]);

		//			events.Clear();
		//		}
		//	}
		//}

		//protected void Shutdown()
		//{
		//	SocketUtilities.CloseSocket(Socket);

		//	if (MultithreadedReceive)
		//		receiveThread.Abort();

		//	if (MultithreadedSend)
		//		sendThread.Abort();
		//}

		//protected void RunReceiveThread()
		//{
		//	if (!MultithreadedReceive)
		//		return;

		//	receiveThread = new Thread(ReceiverWorker);
		//	receiveThread.Start();
		//}

		//protected void RunSenndThread()
		//{
		//	if (!MultithreadedSend)
		//		return;

		//	sendThread = new Thread(SendWorker);
		//	sendThread.Start();
		//}

		//protected abstract void Receive();

		//protected virtual void Send(Socket Target, BufferStream Buffer)
		//{
		//	try
		//	{
		//		lock(Target)
		//		{
		//			if (PacketLossSimulation == 0 || Constants.Random.NextDouble() > PacketLossSimulation)
		//				Target.Send(Buffer.Buffer);

		//			BandwidthOut += Buffer.Size;
		//		}
		//	}
		//	catch (SocketException e)
		//	{
		//		if (e.SocketErrorCode == SocketError.ConnectionReset)
		//		{
		//			HandleDisconnection(Target);

		//			return;
		//		}

		//		throw e;
		//	}
		//	catch (Exception e)
		//	{
		//		throw e;
		//	}
		//}

		//protected abstract bool HandleSendCommand(SendCommand Command);

		//protected abstract void ProcessEvent(EventBase Event);

		//protected void AddEvent(EventBase Event)
		//{
		//	lock(events)
		//		events.Add(Event);
		//}

		//protected void AddSendCommand(SendCommand Command)
		//{
		//	lock(sendCommands)
		//		sendCommands.Add(Command);
		//}

		//protected virtual void HandleDisconnection(Socket Socket)
		//{
		//	//Internationally Left Blank
		//}

		//private void ReceiverWorker()
		//{
		//	while (true)
		//	{
		//		Thread.Sleep(1);

		//		Receive();
		//	}
		//}

		//private void SendWorker()
		//{
		//	while (true)
		//	{
		//		Thread.Sleep(1);

		//		HandleSendCommands();
		//	}
		//}

		//private void HandleSendCommands()
		//{
		//	lock(sendCommands)
		//	{
		//		for (int i = 0; i < sendCommands.Count; ++i)
		//		{
		//			if (!HandleSendCommand(sendCommands[i]))
		//				continue;

		//			sendCommands.RemoveAt(i--);
		//		}
		//	}
		//}

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
		void SetPacketLossSimulation(bool Value)
		{
			m_LatencySimulation = Value;
		}

	private:
		Thread m_ReceiveThread = null;
		Thread m_SendThread = null;

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