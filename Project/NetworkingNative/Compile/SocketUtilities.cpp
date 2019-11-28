// Copyright 2019. All Rights Reserved.
#include "..\Include\SocketUtilities.h"
#include <ws2tcpip.h>

#pragma comment(lib, "Ws2_32.lib")

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
		PlatformNetwork::SetNonBlocking(Socket, Value);
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

	void SocketUtilities::SetBSDUrgentEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::TCP, PlatformNetwork::Options::BSPState, Value);
	}

	// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
	// will apply a bad effect on the send/receive protocol under TCP connection
	// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
	// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
	void SocketUtilities::SetNagleAlgorithmEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::TCP, PlatformNetwork::Options::NoDelay, !Value);
	}

	bool SocketUtilities::IsSocketReady(Socket Socket)
	{
		return !(PlatformNetwork::AvailableBytes(Socket) == 0);
	}

	void SocketUtilities::Bind(Socket Socket, const IPEndPoint& EndPoint)
	{
		PlatformNetwork::Bind(Socket, EndPoint.GetAddress().GetAddressFamily(), EndPoint.GetAddress().GetIP().c_str(), EndPoint.GetPort());
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