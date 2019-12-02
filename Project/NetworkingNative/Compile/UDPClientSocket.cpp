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
	}

	void UDPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
	}
}