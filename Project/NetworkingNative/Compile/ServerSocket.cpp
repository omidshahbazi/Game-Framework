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
		Bind(SocketUtilities::ResolveDomain(Host), Port);
	}

	void ServerSocket::Bind(const IPAddress& IP, uint16_t Port)
	{
		Bind(IPEndPoint(IP, Port));
	}

	void ServerSocket::Bind(const IPEndPoint& EndPoint)
	{
		IPEndPoint endPoint = EndPoint;

		if (endPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			endPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(endPoint.GetAddress()));

		SocketUtilities::Bind(GetSocket(), endPoint);
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