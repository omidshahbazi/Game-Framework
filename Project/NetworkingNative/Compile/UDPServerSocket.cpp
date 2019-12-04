// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPServerSocket.h"

namespace GameFramework::Networking
{
	UDPServerSocket::UDPServerSocket(void) :
		ServerSocket(PlatformNetwork::IPProtocols::UDP)
	{
	}

	void UDPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
	}
}