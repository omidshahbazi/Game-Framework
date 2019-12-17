// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPClientSocket.h"
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"

namespace GameFramework::Networking
{
	UDPClientSocket::UDPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::UDP),
		m_MTU(0)
	{
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

	void UDPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
	}
}