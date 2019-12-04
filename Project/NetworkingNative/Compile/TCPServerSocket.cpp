// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPServerSocket.h"

namespace GameFramework::Networking
{
	TCPServerSocket::TCPServerSocket(uint32_t MaxConnection) :
		ServerSocket(PlatformNetwork::IPProtocols::TCP),
		m_MaxConnection(MaxConnection)
	{
	}

	void TCPServerSocket::Listen(void)
	{
		SocketUtilities::Listen(GetSocket(), m_MaxConnection);

		ServerSocket::Listen();
	}

	void TCPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
		HandleReceivedBuffer(Sender, Buffer);
	}
}