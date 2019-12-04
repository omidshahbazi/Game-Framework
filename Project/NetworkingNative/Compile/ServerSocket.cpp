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

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Length)
	{
		Send(Target, Buffer, 0, Length);
	}

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Constants::Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		AddSendCommand(const_cast<Client*>(Target), buffer);
	}

	void ServerSocket::AddSendCommand(Client* Target, const BufferStream& Buffer)
	{
		BaseSocket::AddSendCommand(new ServerSendCommand(Target, Buffer, GetTimestamp()));
	}

	void ServerSocket::Receive(void)
	{
		AcceptClients();

		ReadFromClients();
	}

	void ServerSocket::HandleIncommingBuffer(Client* Client, BufferStream& Buffer)
	{
		byte control = Buffer.ReadByte();

		double time = Time::GetCurrentEpochTime();

		Client->UpdateLastTouchTime(time);

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Constants::Packet::CreateIncommingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(Client, buffer);
		}
		else if (control == Constants::Control::PING)
		{
			double sendTime = Buffer.ReadFloat64();

			Client->UpdateLatency(abs(time - sendTime) * 1000);

			BufferStream pingBuffer = Constants::Packet::CreatePingBufferStream();

			SendOverSocket(Client, pingBuffer);
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