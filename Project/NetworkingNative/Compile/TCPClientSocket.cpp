// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPClientSocket.h"

namespace GameFramework::Networking
{
	TCPClientSocket::TCPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::TCP)
	{
	}

	void TCPClientSocket::Service(void)
	{
		if (SocketUtilities::poll)
	}

	void TCPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		SocketUtilities::Connect(GetSocket(), EndPoint);
	}

	void TCPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
		HandleReceivedBuffer(Buffer);
	}
}