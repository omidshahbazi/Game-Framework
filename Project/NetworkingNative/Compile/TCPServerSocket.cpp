// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPServerSocket.h"

namespace GameFramework::Networking
{
	TCPServerSocket::TCPServerSocket(uint32_t MaxConnection) :
		ServerSocket(PlatformNetwork::IPProtocols::TCP, MaxConnection)
	{
	}

	void TCPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
		HandleReceivedBuffer(Sender, Buffer);
	}
}