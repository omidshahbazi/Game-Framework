// Copyright 2019. All Rights Reserved.
#include "..\Include\ServerSocket.h"

namespace GameFramework::Networking
{
	ServerSocket::ServerSocket(PlatformNetwork::IPProtocols Type, uint32_t MaxConnection) :
		BaseSocket(Type)
	{
	}

	void ServerSocket::Bind(const std::string& Host, uint16_t Port)
	{
		SocketUtilities::ResolveDomain(Host);
	}

	void ServerSocket::Receive(void)
	{

	}

	bool ServerSocket::HandleSendCommand(const SendCommand& Command)
	{
		return false;
	}

	void ServerSocket::ProcessEvent(const EventBase& Event)
	{

	}

	bool ServerSocket::GetIsReady(void)
	{
		return false;
	}

	double ServerSocket::GetTimestamp(void)
	{
		return 0;
	}
}