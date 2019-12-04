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
	ServerSocket::ServerSocket(PlatformNetwork::IPProtocols Type) :
		BaseSocket(Type),
		m_IsBound(false)
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