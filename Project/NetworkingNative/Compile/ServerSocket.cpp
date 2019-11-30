// Copyright 2019. All Rights Reserved.
#include "..\Include\ServerSocket.h"
#include "..\Include\Constants.h"
#include "..\Include\CallbackUtilities.h"
#include <Timing\Time.h>
#include <Utilities\TypeHelper.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	ServerSocket::ServerSocket(PlatformNetwork::IPProtocols Type, uint32_t MaxConnection) :
		BaseSocket(Type),
		m_IsBound(false),
		m_MaxConnection(MaxConnection)
	{
	}

	void ServerSocket::Bind(const std::string& Host, uint16_t Port)
	{
		Bind(SocketUtilities::ResolveDomain(Host), Port);
	}

	void ServerSocket::Bind(const IPAddress& IP, uint16_t Port)
	{
		Bind(IPEndPoint(IP, Port));
	}

	void ServerSocket::Bind(const IPEndPoint& EndPoint)
	{
		IPEndPoint endPoint = EndPoint;

		if (endPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			endPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(endPoint.GetAddress()));

		SocketUtilities::Bind(GetSocket(), endPoint);
		m_IsBound = true;
	}

	void ServerSocket::ServerSocket::UnBind(void)
	{
		m_Clients.clear();

		Shutdown();

		m_IsBound = false;
	}

	void ServerSocket::DisconnectClient(Client* const Client)
	{
		WAIT_FOR_BOOL(m_ClientsLock);

		m_Clients.remove(Client);

		HandleClientDisconnection(Client);
	}

	void ServerSocket::Listen()
	{
		SocketUtilities::Listen(GetSocket(), m_MaxConnection);

		RunReceiveThread();
		RunSenndThread();
	}

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Length)
	{
		Send(Target, Buffer, 0, Length);
	}

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Constants::Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		Send(Target, buffer);
	}

	void ServerSocket::Send(const Client* Target, const BufferStream& Buffer)
	{
		AddSendCommand(new ServerSendCommand(Target->GetSocket(), Buffer, GetTimestamp()));
	}

	void ServerSocket::Receive(void)
	{
		try
		{
			Socket clientSocket = 0;
			IPEndPoint endPoint;
			if (SocketUtilities::Accept(GetSocket(), clientSocket, endPoint))
			{
				Client* client = new Client(clientSocket, endPoint);

				{
					WAIT_FOR_BOOL(m_ClientsLock);

					m_Clients.push_back(client);
				}

				if (GetMultithreadedCallbacks())
				{
					CallbackUtilities::InvokeCallback(OnClientConnected, reinterpret_cast<const Client*>(client));
				}
				else
				{
					AddEvent(new ClientConnectedEvent(client));
				}
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

		WAIT_FOR_BOOL(m_ClientsLock);
		ClientList disconnectedClients;

		std::byte* receiveBuffer = GetReceiveBuffer();

		for (auto client : m_Clients)
		{
			try
			{
				Socket clientSocket = client->GetSocket();

				uint32_t size = 0;

				if (SocketUtilities::GetAvailableBytes(clientSocket) == 0)
				{
					if (!SocketUtilities::IsReady(clientSocket))
					{
						disconnectedClients.push_back(client);

						HandleClientDisconnection(client);
					}

					continue;
				}

				size = Constants::RECEIVE_BUFFER_SIZE;
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

					HandleClientDisconnection(client);

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

			delete client;
		}
	}

	void ServerSocket::HandleIncommingBuffer(Client* Client, BufferStream& Buffer)
	{
		byte control = Buffer.ReadByte();

		double time = Time::GetCurrentEpochTime();

		Client->UpdateLastTouchTime(time);

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Constants::Packet::CreateIncommingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			if (GetMultithreadedCallbacks())
			{
				CallbackUtilities::InvokeCallback(OnBufferReceived, reinterpret_cast<const Networking::Client*>(Client), buffer);
			}
			else
			{
				AddEvent(new BufferReceivedvent(Client, buffer));
			}
		}
		else if (control == Constants::Control::PING)
		{
			double sendTime = Buffer.ReadFloat64();

			Client->UpdateLatency((time - sendTime) * 1000);

			BufferStream pingBuffer = Constants::Packet::CreatePingBufferStream();

			BaseSocket::Send(Client->GetSocket(), pingBuffer);
		}
	}

	bool ServerSocket::HandleSendCommand(SendCommand* Command)
	{
		if (GetTimestamp() < Command->GetSendTime() + (GetLatencySimulation() / 1000.0F))
			return false;

		ServerSendCommand* sendCommand = reinterpret_cast<ServerSendCommand*>(Command);

		if (!SocketUtilities::IsReady(sendCommand->GetSocket()))
			return false;

		BaseSocket::Send(sendCommand->GetSocket(), Command->GetBuffer());

		return true;
	}

	void ServerSocket::ProcessEvent(EventBase* Event)
	{
		ServerEventBase* ev = reinterpret_cast<ServerEventBase*>(Event);

		if (IS_TYPE_OF(ev, ClientConnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnClientConnected, ev->GetClient());
		}
		else if (IS_TYPE_OF(ev, ClientDisconnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnClientDisconnected, ev->GetClient());

			SocketUtilities::CloseSocket(ev->GetClient()->GetSocket());
		}
		else if (IS_TYPE_OF(ev, BufferReceivedvent))
		{
			ProcessReceivedBuffer(const_cast<Client*>(ev->GetClient()), reinterpret_cast<BufferReceivedvent*>(Event)->GetBuffer());
		}
	}

	void ServerSocket::HandleReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnBufferReceived, reinterpret_cast<const Client*>(Sender), Buffer);
		}
		else
		{
			AddEvent(new BufferReceivedvent(Sender, Buffer));
		}
	}

	double ServerSocket::GetTimestamp(void)
	{
		return Time::GetCurrentEpochTime();
	}

	void ServerSocket::HandleClientDisconnection(Client* Client)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnClientDisconnected, reinterpret_cast<const Networking::Client*>(Client));

			SocketUtilities::CloseSocket(Client->GetSocket());
		}
		else
		{
			AddEvent(new ClientDisconnectedEvent(Client));
		}
	}
}