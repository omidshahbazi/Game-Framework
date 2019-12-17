// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPClientSocket.h"
#include "..\Include\Constants.h"

namespace GameFramework::Networking
{
	UDPClientSocket::UDPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::UDP)
	{
	}

	void UDPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		uint32_t mtu = SocketUtilities::FindOptimumMTU(EndPoint.GetAddress(), 1000, Constants::UDP::MAX_MTU);
	}

	void UDPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
	}
}