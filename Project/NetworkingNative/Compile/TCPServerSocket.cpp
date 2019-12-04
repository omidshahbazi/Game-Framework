// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPServerSocket.h"
#include "..\Include\Constants.h"

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

	void TCPServerSocket::SendOverSocket(Client* Client, const BufferStream& Buffer)
	{
		BaseSocket::SendOverSocket(reinterpret_cast<TCPClient*>(Client)->GetSocket(), Buffer);
	}

	void TCPServerSocket::Listen(void)
	{
		SocketUtilities::Listen(GetSocket(), m_MaxConnection);

		ServerSocket::Listen();
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

				if (SocketUtilities::GetAvailableBytes(clientSocket) == 0)
				{
					if (!client->GetIsReady())
						disconnectedClients.push_back(client);

					continue;
				}

				uint32_t size = Constants::RECEIVE_BUFFER_SIZE;
				if (!SocketUtilities::Receive(clientSocket, receiveBuffer, size))
					continue;

				AddBandwidthIn(size);

				uint32_t index = 0;
				while (index != size)
				{
					uint32_t packetSize = *(reinterpret_cast<uint32_t*>(receiveBuffer + index));

					index += Constants::Packet::PACKET_SIZE_SIZE;

					BufferStream buffer = BufferStream(receiveBuffer, index, packetSize);

					HandleIncommingBuffer(client, buffer);

					index += packetSize;
				}
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

	bool TCPServerSocket::HandleSendCommand(SendCommand* Command)
	{
		ServerSendCommand* sendCommand = reinterpret_cast<ServerSendCommand*>(Command);
		TCPClient* client = reinterpret_cast<TCPClient*>(sendCommand->GetClient());

		if (!client->GetIsReady())
			return false;

		BaseSocket::SendOverSocket(client->GetSocket(), Command->GetBuffer());

		return true;
	}

	void TCPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
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