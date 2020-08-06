// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPServerSocket.h"
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	TCPServerSocket::TCPServerSocket(uint32_t MaxConnection) :
		ServerSocket(PlatformNetwork::IPProtocols::TCP),
		m_MaxConnection(MaxConnection)
	{
	}

	void TCPServerSocket::UnBind(void)
	{
		m_Clients.clear();

		ServerSocket::UnBind();
	}

	void TCPServerSocket::DisconnectClient(Client* const Client)
	{
		WAIT_FOR_BOOL(m_ClientsLock);

		m_Clients.remove(reinterpret_cast<TCPClient*>(Client));

		ServerSocket::DisconnectClient(Client);
	}

	void TCPServerSocket::Listen(void)
	{
		SocketUtilities::Listen(GetSocket(), m_MaxConnection);

		ServerSocket::Listen();
	}

	void TCPServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Length)
	{
		Send(Target, Buffer, 0, Length);
	}

	void TCPServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		AddSendCommand(const_cast<Client*>(Target), buffer);
	}

	void TCPServerSocket::AddSendCommand(Client* Target, const BufferStream& Buffer)
	{
		BaseSocket::AddSendCommand(new ServerSendCommand(Target, Buffer, GetTimestamp()));
	}

	void TCPServerSocket::AcceptClients(void)
	{
		try
		{
			Socket clientSocket = 0;
			IPEndPoint endPoint(IPAddress::AnyV6, 0);
			if (SocketUtilities::Accept(GetSocket(), clientSocket, endPoint))
			{
				TCPClient* client = new TCPClient(clientSocket, endPoint);
				client->GetStatistics().SetPacketCountRate(GetPacketCountRate());

				{
					WAIT_FOR_BOOL(m_ClientsLock);

					m_Clients.push_back(client);
				}

				RaiseOnClientConnected(client);
			}
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.GetError() != PlatformNetwork::Errors::WouldBlock)
				throw e;
		}
		catch (exception e)
		{
			throw e;
		}
	}

	void TCPServerSocket::ReadFromClients(void)
	{
		WAIT_FOR_BOOL(m_ClientsLock);
		TCPClientList disconnectedClients;

		std::byte* receiveBuffer = GetReceiveBuffer();

		for (auto client : m_Clients)
		{
			try
			{
				Socket clientSocket = client->GetSocket();

				uint32_t availableSize = SocketUtilities::GetAvailableBytes(clientSocket);
				if (availableSize == 0)
				{
					if (!client->GetIsReady())
						disconnectedClients.push_back(client);

					continue;
				}

				uint32_t receiveSize = availableSize;
				if (!SocketUtilities::Receive(clientSocket, receiveBuffer, GetReceiveBufferIndex(), receiveSize))
					continue;

				ServerSocket::ProcessReceivedBuffer(client, receiveSize);
			}
			catch (PlatformNetwork::SocketException e)
			{
				if (e.GetError() == PlatformNetwork::Errors::WouldBlock)
					continue;
				if (e.GetError() == PlatformNetwork::Errors::ConnectionReset)
				{
					disconnectedClients.push_back(client);

					continue;
				}

				throw e;
			}
			catch (exception e)
			{
				throw e;
			}
		}

		for (auto client : disconnectedClients)
		{
			m_Clients.remove(client);

			HandleClientDisconnection(client);
		}
	}

	void TCPServerSocket::HandleIncomingBuffer(Client* Client, BufferStream& Buffer)
	{
		byte control = Buffer.ReadByte();

		double time = Time::GetCurrentEpochTime();

		Client->GetStatistics().SetLastTouchTime(time);

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Packet::CreateIncomingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(Client, buffer);
		}
		else if (control == Constants::Control::PING)
		{
			double sendTime = Buffer.ReadFloat64();

			Client->GetStatistics().SetLatency(abs(time - sendTime) * 1000);

			BufferStream pingBuffer = Packet::CreatePingBufferStream();

			AddSendCommand(Client, pingBuffer);
		}
	}

	bool TCPServerSocket::HandleSendCommand(SendCommand* Command)
	{
		ServerSendCommand* sendCommand = reinterpret_cast<ServerSendCommand*>(Command);
		TCPClient* client = reinterpret_cast<TCPClient*>(sendCommand->GetClient());

		//if (!client->GetIsReady())
		//	return false;

		client->GetStatistics().AddBandwidthOut(Command->GetBuffer().GetSize());

		client->GetStatistics().SetLastTouchTime(Time::GetCurrentEpochTime());

		if (!BaseSocket::SendOverSocket(client->GetSocket(), Command->GetBuffer()))
		{
			HandleClientDisconnection(client);

			WAIT_FOR_BOOL(m_ClientsLock);
			m_Clients.remove(client);

			return false;
		}

		return true;
	}

	void TCPServerSocket::ProcessReceivedBuffer(Client* Sender, BufferStream& Buffer)
	{
		HandleReceivedBuffer(Sender, Buffer);
	}

	void TCPServerSocket::CloseClientConnection(Client* Client)
	{
		ServerSocket::CloseClientConnection(Client);

		SocketUtilities::CloseSocket(reinterpret_cast<TCPClient*>(Client)->GetSocket());

		delete Client;
	}
}