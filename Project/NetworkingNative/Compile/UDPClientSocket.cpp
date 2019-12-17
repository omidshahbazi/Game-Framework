// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPClientSocket.h"
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	UDPClientSocket::UDPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::UDP),
		m_MTU(0)
	{
	}

	void UDPClientSocket::Send(byte* const Buffer, uint32_t Length, bool Reliable)
	{
		Send(Buffer, 0, Length, Reliable);
	}

	void UDPClientSocket::Send(byte* const Buffer, uint32_t Index, uint32_t Length, bool Reliable)
	{
		//BufferStream buffer = Packet::CreateOutgoingBufferStream(Length);

		//buffer.WriteBytes(Buffer, Index, Length);

		//SendInternal(buffer);
	}

	void UDPClientSocket::SendInternal(BufferStream& Buffer)
	{
		AddSendCommand(new SendCommand(Buffer, GetTimestamp()));
	}

	void UDPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		SocketUtilities::Connect(GetSocket(), EndPoint);

		m_MTU = SocketUtilities::FindOptimumMTU(EndPoint.GetAddress(), 1000, Constants::UDP::MAX_MTU);

		BufferStream buffer = Packet::CreateHandshakeBufferStream(m_MTU);
		SendInternal(buffer);

		RunReceiveThread();
		RunSenndThread();
	}

	void UDPClientSocket::HandleIncomingBuffer(BufferStream& Buffer)
	{
		GetStatistics().SetLastTouchTime(Time::GetCurrentEpochTime());

		byte control = Buffer.ReadByte();

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Packet::CreateIncomingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(buffer);
		}
		else if (control == Constants::Control::HANDSHAKE_BACK)
		{
			GetStatistics().SetPacketCountRate(Buffer.ReadUInt32());

			SetIsConnected(true);

			RaiseOnConnectedEvent();
		}
		else if (control == Constants::Control::PING)
		{
			HandlePingPacket(Buffer);
		}
	}

	void UDPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
	}
}