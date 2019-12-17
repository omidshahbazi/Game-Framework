// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPClientSocket.h"
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include "..\Include\CallbackUtilities.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	TCPClientSocket::TCPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::TCP),
		m_IsConnecting(false)
	{
	}

	void TCPClientSocket::Send(byte* const Buffer, uint32_t Length)
	{
		Send(Buffer, 0, Length);
	}

	void TCPClientSocket::Send(byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		SendInternal(buffer);
	}

	void TCPClientSocket::Service(void)
	{
		ClientSocket::Service();

		if (m_IsConnecting)
			CheckConnectionStatus();
	}

	void TCPClientSocket::SendInternal(const BufferStream& Buffer)
	{
		AddSendCommand(new SendCommand(Buffer, GetTimestamp()));
	}

	void TCPClientSocket::Disconnect(void)
	{
		ClientSocket::Disconnect();

		m_IsConnecting = false;
	}

	void TCPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		m_IsConnecting = true;

		SocketUtilities::Connect(GetSocket(), EndPoint);
	}

	void TCPClientSocket::Receive(void)
	{
		if (!GetIsConnected())
			return;

		ClientSocket::Receive();
	}

	void TCPClientSocket::HandleIncomingBuffer(BufferStream& Buffer)
	{
		GetStatistics().SetLastTouchTime(Time::GetCurrentEpochTime());

		byte control = Buffer.ReadByte();

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Packet::CreateIncomingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(buffer);
		}
		else if (control == Constants::Control::PING)
		{
			HandlePingPacket(Buffer);
		}
	}

	void TCPClientSocket::ProcessReceivedBuffer(BufferStream& Buffer)
	{
		HandleReceivedBuffer(Buffer);
	}

	BufferStream TCPClientSocket::GetPingPacket(void)
	{
		return Packet::CreatePingBufferStream();
	}

	void TCPClientSocket::CheckConnectionStatus(void)
	{
		if (!GetIsConnected())
		{
			try
			{
				if (SocketUtilities::Select(GetSocket(), PlatformNetwork::SelectModes::SelectWrite, 1000))
				{
					SetIsConnected(true);
					m_IsConnecting = false;

					RunReceiveThread();
					RunSenndThread();

					RaiseOnConnectedEvent();
				}
			}
			catch (PlatformNetwork::SocketException e)
			{
				m_IsConnecting = false;

				RaiseOnConnectionFailedEvent();
			}
		}
	}
}