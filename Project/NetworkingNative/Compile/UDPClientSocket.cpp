// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPClientSocket.h"

namespace GameFramework::Networking
{
	UDPClientSocket::UDPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::UDP)
	{
	}

	void UDPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		SocketUtilities::FindOptimumMTU(IPAddress(PlatformNetwork::AddressFamilies::InterNetwork, "8.8.8.8"), 1000, 1500);
		//SocketUtilities::FindOptimumMTU(EndPoint.GetAddress(), 1000, 1500);
	}

	void UDPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
	}
}