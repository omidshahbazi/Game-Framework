// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPServerSocket.h"

namespace GameFramework::Networking
{
	UDPServerSocket::UDPServerSocket(uint32_t MaxConnection) :
		ServerSocket(PlatformNetwork::IPProtocols::UDP, MaxConnection)
	{
	}

	void UDPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
	}
}