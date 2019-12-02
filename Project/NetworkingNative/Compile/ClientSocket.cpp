// Copyright 2019. All Rights Reserved.
#include "..\Include\ClientSocket.h"
#include "..\Include\Constants.h"
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
		m_TimeOffset(0),
		m_LastTouchTime(0),
		m_Latency(0)
	{
	}

	void ClientSocket::Service(void)
	{
		if (m_IsConnected && m_LastTouchTime + Constants::PING_TIME <= Time::GetCurrentEpochTime())
		{
			m_LastTouchTime = Time::GetCurrentEpochTime();

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
		IPEndPoint endPoint = EndPoint;

		if (endPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			endPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(endPoint.GetAddress()));

		ConnectInternal(EndPoint);
	}

	void ClientSocket::Disconnect(void)
	{
		Shutdown();
	}

	void ClientSocket::Send(byte* const Buffer, uint32_t Length)
	{
		Send(Buffer, 0, Length);
	}

	void ClientSocket::Send(byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Constants::Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		SendInternal(buffer);
	}

	void ClientSocket::SendInternal(const BufferStream& Buffer)
	{
		AddSendCommand(new SendCommand(Buffer, GetTimestamp()));
	}

	void ClientSocket::Receive(void)
	{
		if (!m_IsConnected)
			return;

		Socket socket = GetSocket();

		try
		{
			if (SocketUtilities::GetAvailableBytes(socket) == 0)
			{
				if (!GetIsReady())
					HandleDisconnection(socket);

				return;
			}

			std::byte* receiveBuffer = GetReceiveBuffer();

			uint32_t size = Constants::RECEIVE_BUFFER_SIZE;
			if (!SocketUtilities::Receive(socket, receiveBuffer, size))
				return;

			AddBandwidthIn(size);

			uint32_t index = 0;
			while (index != size)
			{
				uint32_t packetSize = *(reinterpret_cast<uint32_t*>(receiveBuffer + index));

				index += Constants::Packet::PACKET_SIZE_SIZE;

				BufferStream buffer = BufferStream(receiveBuffer, index, packetSize);

				HandleIncommingBuffer(buffer);

				index += packetSize;
			}
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

	void ClientSocket::HandleIncommingBuffer(BufferStream& Buffer)
	{
		double time = Time::GetCurrentEpochTime();

		m_LastTouchTime = time;

		byte control = Buffer.ReadByte();

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Constants::Packet::CreateIncommingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			if (GetMultithreadedCallbacks())
			{
				CallbackUtilities::InvokeCallback(OnBufferReceived, buffer);
			}
			else
			{
				AddEvent(new BufferReceivedvent(buffer));
			}
		}
		else if (control == Constants::Control::PING)
		{
			double sendTime = Buffer.ReadFloat64();

			m_Latency = (time - sendTime) * 1000;

			double t0 = m_LastPingTime;
			double t1 = sendTime;
			double t2 = sendTime;
			double t3 = time;

			m_TimeOffset = ((t1 - t0) + (t2 - t3)) / 2;
		}
	}

	bool ClientSocket::HandleSendCommand(SendCommand* Command)
	{
		if (GetTimestamp() < Command->GetSendTime() + (GetLatencySimulation() / 1000.0F))
			return false;

		if (!GetIsReady())
			return false;

		BaseSocket::SendInternal(GetSocket(), const_cast<BufferStream&>(Command->GetBuffer()));

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

	void ClientSocket::SendPing(void)
	{
		BufferStream pingBuffer = Constants::Packet::CreatePingBufferStream();

		m_LastPingTime = Time::GetCurrentEpochTime();

		BaseSocket::SendInternal(GetSocket(), pingBuffer);
	}
}