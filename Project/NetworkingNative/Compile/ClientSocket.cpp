// Copyright 2019. All Rights Reserved.
#include "..\Include\ClientSocket.h"
#include "..\Include\Constants.h"
#include "..\Include\Packet.h"
#include "..\Include\CallbackUtilities.h"
#include <Timing\Time.h>
#include <Utilities\TypeHelper.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	ClientSocket::ClientSocket(PlatformNetwork::IPProtocols Type) :
		BaseSocket(Type),
		m_IsConnected(false),
		m_LastPingTime(0),
		m_TimeOffset(0)
	{
	}

	void ClientSocket::Service(void)
	{
		if (m_IsConnected && GetStatistics().GetLastTouchTime() + Constants::PING_TIME <= Time::GetCurrentEpochTime())
		{
			GetStatistics().SetLastTouchTime(Time::GetCurrentEpochTime());

			SendPing();
		}

		BaseSocket::Service();
	}

	void ClientSocket::Connect(const std::string& Host, uint16_t Port)
	{
		Connect(SocketUtilities::ResolveDomain(Host), Port);
	}

	void ClientSocket::Connect(const IPAddress& IP, uint16_t Port)
	{
		Connect(IPEndPoint(IP, Port));
	}

	void ClientSocket::Connect(const IPEndPoint& EndPoint)
	{
		m_RemoteEndPoint = EndPoint;

		if (m_RemoteEndPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			m_RemoteEndPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(m_RemoteEndPoint.GetAddress()));

		ConnectInternal(m_RemoteEndPoint);
	}

	void ClientSocket::Disconnect(void)
	{
		Shutdown();
	}

	void ClientSocket::Receive(void)
	{
		Socket socket = GetSocket();

		try
		{
			int availableSize = SocketUtilities::GetAvailableBytes(socket);

			if (availableSize == 0)
			{
				if (!GetIsReady())
					HandleDisconnection(socket);

				return;
			}

			std::byte* receiveBuffer = GetReceiveBuffer();

			uint32_t receiveBufferIndex = GetReceiveBufferIndex();

			uint32_t receiveSize = availableSize;
			if (!SocketUtilities::Receive(socket, receiveBuffer, receiveBufferIndex, receiveSize))
				return;

			GetStatistics().AddBandwidthIn(receiveSize);

			uint32_t totalSize = receiveBufferIndex + receiveSize;

			uint32_t index = 0;
			while (index != totalSize)
			{
				uint32_t packetSize = *(reinterpret_cast<uint32_t*>(receiveBuffer + index));

				if (totalSize < packetSize)
				{
					SetReceiveBufferIndex(totalSize);
					return;
				}

				index += Packet::PACKET_SIZE_SIZE;

				BufferStream buffer = BufferStream(receiveBuffer, index, packetSize);

				HandleIncomingBuffer(buffer);

				index += packetSize;
			}

			SetReceiveBufferIndex(0);
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.GetError() == PlatformNetwork::Errors::ConnectionReset)
			{
				HandleDisconnection(socket);

				return;
			}

			throw e;
		}
	}

	bool ClientSocket::HandleSendCommand(SendCommand* Command)
	{
		if (!GetIsReady())
			return false;

		BaseSocket::SendOverSocket(GetSocket(), const_cast<BufferStream&>(Command->GetBuffer()));

		return true;
	}

	void ClientSocket::ProcessEvent(EventBase* Event)
	{
		ClientEventBase* ev = dynamic_cast<ClientEventBase*>(Event);

		if (IS_TYPE_OF(ev, ConnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnConnected);
		}
		else if (IS_TYPE_OF(ev, ConnectionFailedEvent))
		{
			CallbackUtilities::InvokeCallback(OnConnectionFailed);
		}
		else if (IS_TYPE_OF(ev, DisconnectedEvent))
		{
			CallbackUtilities::InvokeCallback(OnDisconnected);
		}
		else if (IS_TYPE_OF(ev, BufferReceivedvent))
		{
			CallbackUtilities::InvokeCallback(OnBufferReceived, dynamic_cast<BufferReceivedvent*>(Event)->GetBuffer());
		}
	}

	void ClientSocket::HandleDisconnection(Socket Socket)
	{
		BaseSocket::HandleDisconnection(Socket);

		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnDisconnected);
		}
		else
		{
			AddEvent(new DisconnectedEvent());
		}

		m_IsConnected = false;
	}

	void ClientSocket::HandleReceivedBuffer(const BufferStream& Buffer)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnBufferReceived, Buffer);
		}
		else
		{
			AddEvent(new BufferReceivedvent(Buffer));
		}
	}

	double ClientSocket::GetTimestamp(void) const
	{
		return Time::GetCurrentEpochTime() + m_TimeOffset;
	}

	void ClientSocket::RaiseOnConnectedEvent(void)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnConnected);
		}
		else
		{
			AddEvent(new ConnectedEvent());
		}
	}

	void ClientSocket::RaiseOnConnectionFailedEvent(void)
	{
		if (GetMultithreadedCallbacks())
		{
			CallbackUtilities::InvokeCallback(OnConnectionFailed);
		}
		else
		{
			AddEvent(new ConnectionFailedEvent());
		}
	}

	void ClientSocket::HandlePingPacket(BufferStream& Buffer)
	{
		double time = Time::GetCurrentEpochTime();

		double sendTime = Buffer.ReadFloat64();

		GetStatistics().SetLatency((time - sendTime) * 1000);

		double t0 = m_LastPingTime;
		double t1 = sendTime;
		double t2 = sendTime;
		double t3 = time;

		m_TimeOffset = ((t1 - t0) + (t2 - t3)) / 2;

		uint32_t payloadSize = Buffer.GetSize() - Packet::PING_SIZE;
		if (payloadSize != 0)
		{
			BufferStream buffer(Buffer.GetBuffer(), Packet::PING_SIZE - Packet::PACKET_SIZE_SIZE, payloadSize);
			HandlePingPacketPayload(buffer);
		}
	}

	void ClientSocket::SendPing(void)
	{
		BufferStream pingBuffer = GetPingPacket();

		m_LastPingTime = Time::GetCurrentEpochTime();

		BaseSocket::SendOverSocket(GetSocket(), pingBuffer);
	}
}