// Copyright 2019. All Rights Reserved.
#include "..\Include\BaseSocket.h"
#include "..\Include\Constants.h"

namespace GameFramework::Networking
{
	void GlobalReceiverWorker(BaseSocket* Instance)
	{
		Instance->ReceiverWorker();
	}

	void GlobalSendWorker(BaseSocket* Instance)
	{
		Instance->SendWorker();
	}

	BaseSocket::BaseSocket(PlatformNetwork::IPProtocols Type) :
		m_ReceiveThread(nullptr),
		m_SendThread(nullptr),
		m_EventsLock(0),
		m_Socket(0),
		m_ReceiveBuffer(nullptr),
		m_BandwidthIn(0),
		m_BandwidthOut(0),
		m_MultithreadedCallbacks(true),
		m_MultithreadedReceive(true),
		m_MultithreadedSend(true),
		m_PacketLossSimulation(0),
		m_LatencySimulation(0)
	{
		m_Socket = SocketUtilities::CreateSocket(Type);
		SocketUtilities::SetBlocking(m_Socket, false);
		SocketUtilities::SetReceiveBufferSize(m_Socket, Constants::RECEIVE_BUFFER_SIZE);
		SocketUtilities::SetSendBufferSize(m_Socket, Constants::SEND_BUFFER_SIZE);
		SocketUtilities::SetReceiveTimeout(m_Socket, Constants::RECEIVE_TIMEOUT);
		SocketUtilities::SetSendTimeout(m_Socket, Constants::SEND_TIMEOUT);
		SocketUtilities::SetTimeToLive(m_Socket, Constants::TIME_TO_LIVE);
		SocketUtilities::SetIPv6OnlyEnabled(m_Socket, false);
		//SocketUtilities::SetChecksumEnabled(m_Socket, false);
		SocketUtilities::SetNagleAlgorithmEnabled(m_Socket, false);

		m_ReceiveBuffer = new std::byte[Constants::RECEIVE_BUFFER_SIZE];

		m_MultithreadedCallbacks = true;
		m_MultithreadedReceive = true;
		m_MultithreadedSend = true;
	}

	void BaseSocket::Service(void)
	{
		if (!m_MultithreadedReceive && GetIsReady())
			Receive();

		if (!m_MultithreadedSend && GetIsReady())
			HandleSendCommands();

		if (!m_MultithreadedCallbacks)
		{
			WAIT_FOR_BOOL(m_EventsLock);

			for (EventBase* ev : m_Events)
				ProcessEvent(ev);

			m_Events.clear();
		}
	}

	void BaseSocket::Shutdown(void)
	{
		SocketUtilities::CloseSocket(m_Socket);

		if (m_MultithreadedReceive)
			m_ReceiveThread->detach();

		if (m_MultithreadedSend)
			m_SendThread->detach();
	}

	void BaseSocket::RunReceiveThread(void)
	{
		if (!m_MultithreadedReceive)
			return;

		m_ReceiveThread = new thread(GlobalReceiverWorker, this);
	}

	void BaseSocket::RunSenndThread(void)
	{
		if (!m_MultithreadedSend)
			return;

		m_SendThread = new thread(GlobalSendWorker, this);
	}

	void BaseSocket::Send(Socket Target, const BufferStream& Buffer)
	{
		try
		{
			if (m_PacketLossSimulation == 0 || Constants::Random.NextDouble() > m_PacketLossSimulation)
				SocketUtilities::Send(Target, Buffer.GetBuffer(), Buffer.GetSize());

			m_BandwidthOut += Buffer.GetSize();
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.GetError() == PlatformNetwork::Errors::ConnectionReset)
			{
				HandleDisconnection(Target);

				return;
			}

			throw e;
		}
	}

	void BaseSocket::AddEvent(EventBase* Event)
	{
		WAIT_FOR_BOOL(m_EventsLock);

		m_Events.push_back(Event);
	}

	void BaseSocket::AddSendCommand(SendCommand* Command)
	{
		WAIT_FOR_BOOL(m_SendCommandsLock);

		m_SendCommands.push_back(Command);
	}

	void BaseSocket::HandleDisconnection(Socket Socket)
	{
		//Internationally Left Blank
	}

	void BaseSocket::ReceiverWorker(void)
	{
		while (true)
		{
			this_thread::sleep_for(chrono::seconds(1));

			Receive();
		}
	}

	void BaseSocket::SendWorker(void)
	{
		while (true)
		{
			this_thread::sleep_for(chrono::seconds(1));

			HandleSendCommands();
		}
	}

	void BaseSocket::HandleSendCommands(void)
	{
		WAIT_FOR_BOOL(m_SendCommandsLock);

		for (int i = 0; i < m_SendCommands.size(); ++i)
		{
			if (!HandleSendCommand(m_SendCommands[i]))
				continue;

			m_SendCommands.erase(m_SendCommands.begin() + i);

			i--;
		}
	}
}