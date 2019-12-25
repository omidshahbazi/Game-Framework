// Copyright 2019. All Rights Reserved.
#include "..\Include\ServerSocket.h"
#include "..\Include\Constants.h"
#include "..\Include\CallbackUtilities.h"
#include "..\Include\Packet.h"
#include <Timing\Time.h>
#include <Utilities\TypeHelper.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	ServerSocket::ServerSocket(PlatformNetwork::IPProtocols Type) :
		BaseSocket(Type),
		m_IsBound(false),
		m_PacketCountRate(Constants::DEFAULT_PACKET_COUNT_RATE)
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
		m_LocalEndPoint = EndPoint;

		if (m_LocalEndPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			m_LocalEndPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(m_LocalEndPoint.GetAddress()));

		SocketUtilities::Bind(GetSocket(), m_LocalEndPoint);
		m_IsBound = true;
	}

	void ServerSocket::ServerSocket::UnBind(void)
	{
		Shutdown();

		m_IsBound = false;
	}

	void ServerSocket::DisconnectClient(Client* const Client)
	{
		HandleClientDisconnection(Client);
	}

	void ServerSocket::Listen(void)
	{
		RunReceiveThread();
		RunSenndThread();
	}

	void ServerSocket::Receive(void)
	{
		AcceptClients();

		ReadFromClients();
	}

	void ServerSocket::ProcessReceivedBuffer(Client* Client, uint32_t Size)
	{
		std::byte* receiveBuffer = GetReceiveBuffer();

		GetStatistics().AddBandwidthIn(Size);
		Client->GetStatistics().AddBandwidthIn(Size);

		uint32_t index = 0;
		while (index < Size)
		{
			uint32_t packetSize = *(reinterpret_cast<uint32_t*>(receiveBuffer + index));

			if (packetSize > Size)
				throw exception("Incoming packet is invalid");

			index += Packet::PACKET_SIZE_SIZE;

			BufferStream buffer = BufferStream(receiveBuffer, index, packetSize);

			HandleIncomingBuffer(Client, buffer);

			index += packetSize;
		}
	}

	void ServerSocket::ProcessEvent(EventBase* Event)
	{
		ServerEventBase* ev = dynamic_cast<ServerEventBase*>(Event);

		if (IS_TYPE_OF(ev, ClientConnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnClientConnected, ev->GetClient());
		}
		else if (IS_TYPE_OF(ev, ClientDisconnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnClientDisconnected, ev->GetClient());

			CloseClientConnection(const_cast<Client*>(ev->GetClient()));
		}
		else if (IS_TYPE_OF(ev, BufferReceivedvent))
		{
			CallbackUtilities::InvokeCallback(OnBufferReceived, ev->GetClient(), dynamic_cast<BufferReceivedvent*>(Event)->GetBuffer());
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

	void ServerSocket::HandleClientDisconnection(Client* Client)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnClientDisconnected, reinterpret_cast<const Networking::Client*>(Client));

			CloseClientConnection(Client);
		}
		else
		{
			AddEvent(new ClientDisconnectedEvent(Client));
		}
	}

	void ServerSocket::RaiseOnClientConnected(Client* Client)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnClientConnected, reinterpret_cast<const Networking::Client*>(Client));
		}
		else
		{
			AddEvent(new ClientConnectedEvent(Client));
		}
	}

	double ServerSocket::GetTimestamp(void) const
	{
		return Time::GetCurrentEpochTime();
	}
}