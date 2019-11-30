// Copyright 2019. All Rights Reserved.
#include "..\Include\SocketUtilities.h"

using namespace std;

namespace GameFramework::Networking
{
	Socket SocketUtilities::CreateSocket(PlatformNetwork::IPProtocols Protocol)
	{
		PlatformNetwork::Initialize();

		PlatformNetwork::Types type = (Protocol == PlatformNetwork::IPProtocols::TCP ? PlatformNetwork::Types::Stream : PlatformNetwork::Types::Datagram);

		return PlatformNetwork::Create(PlatformNetwork::AddressFamilies::InterNetworkV6, type, Protocol);
	}

	void SocketUtilities::CloseSocket(Socket Socket)
	{
		PlatformNetwork::Close(Socket);
	}

	void SocketUtilities::SetBlocking(Socket Socket, bool Value)
	{
		PlatformNetwork::SetBlocking(Socket, Value);
	}

	void SocketUtilities::SetReceiveBufferSize(Socket Socket, uint32_t Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::Socket, PlatformNetwork::Options::ReceiveBuffer, (int32_t)Value);
	}

	void SocketUtilities::SetSendBufferSize(Socket Socket, uint32_t Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::Socket, PlatformNetwork::Options::SendBuffer, (int32_t)Value);
	}

	void SocketUtilities::SetReceiveTimeout(Socket Socket, uint32_t Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::Socket, PlatformNetwork::Options::ReceiveTimeout, (int32_t)Value);
	}

	void SocketUtilities::SetSendTimeout(Socket Socket, uint32_t Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::Socket, PlatformNetwork::Options::SendTimeout, (int32_t)Value);
	}

	void SocketUtilities::SetTimeToLive(Socket Socket, uint16_t Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::Socket, PlatformNetwork::Options::TimeToLive, (int32_t)Value);
	}

	void SocketUtilities::SetIPv6OnlyEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::IPV6, PlatformNetwork::Options::IPv6Only, (int32_t)Value);
	}

	void SocketUtilities::SetChecksumEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::IPV6, PlatformNetwork::Options::Checksum, Value);
	}

	// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
	// will apply a bad effect on the send/receive protocol under TCP connection
	// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
	// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
	void SocketUtilities::SetNagleAlgorithmEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::TCP, PlatformNetwork::Options::NoDelay, !Value);
	}

	bool SocketUtilities::IsReady(Socket Socket)
	{
		return !(PlatformNetwork::Poll(Socket, 10, PlatformNetwork::SelectModes::SelectRead) && GetAvailableBytes(Socket) == 0);
	}

	void SocketUtilities::Bind(Socket Socket, const IPEndPoint& EndPoint)
	{
		PlatformNetwork::Bind(Socket, EndPoint.GetAddress().GetAddressFamily(), EndPoint.GetAddress().GetIP().c_str(), EndPoint.GetPort());
	}

	void SocketUtilities::Listen(Socket Socket, uint32_t MaxConnections)
	{
		return PlatformNetwork::Listen(Socket, MaxConnections);
	}

	bool SocketUtilities::Accept(Socket ListenerSocket, Socket& AcceptedSocket, IPEndPoint& EndPoint)
	{
		PlatformNetwork::AddressFamilies family = PlatformNetwork::AddressFamilies::InterNetworkV6;
		std::string ip;
		uint16_t port;

		if (!PlatformNetwork::Accept(ListenerSocket, AcceptedSocket, family, ip, port))
			return false;

		EndPoint.SetAddress(IPAddress(family, ip));
		EndPoint.SetPort(port);

		return true;
	}

	void SocketUtilities::Send(Socket Socket, const std::byte* Buffer, uint32_t Length)
	{
		PlatformNetwork::Send(Socket, Buffer, Length, PlatformNetwork::SendModes::None);
	}

	uint64_t SocketUtilities::GetAvailableBytes(Socket Socket)
	{
		return PlatformNetwork::GetAvailableBytes(Socket);
	}

	bool SocketUtilities::Receive(Socket Socket, std::byte* Buffer, uint32_t& Length)
	{
		return PlatformNetwork::Receive(Socket, Buffer, Length, PlatformNetwork::ReceiveModes::None);
	}

	IPAddress SocketUtilities::ResolveDomain(const string& Domain)
	{
		PlatformNetwork::AddressFamilies family;
		std::string address;
		PlatformNetwork::ResolveDomain(Domain, family, address);

		return IPAddress(family, address);
	}

	IPAddress SocketUtilities::MapIPv4ToIPv6(IPAddress IP)
	{
		if (IP.GetAddressFamily() != PlatformNetwork::AddressFamilies::InterNetwork)
			throw new exception("IP must be v4");

		return ResolveDomain("::ffff:" + IP.GetIP());
	}
}