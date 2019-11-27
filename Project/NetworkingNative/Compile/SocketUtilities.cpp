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
		PlatformNetwork::Bind(Socket, EndPoint.GetAddress().GetAddressFamily(), EndPoint.GetAddress().GetIPv4Address(), EndPoint.GetPort());
	}

	IPAddress SocketUtilities::ResolveDomain(const string& Domain)
	{
		ADDRINFO hintInfo;
		ZeroMemory(&hintInfo, sizeof(ADDRINFO));
		hintInfo.ai_flags = 0;
		hintInfo.ai_family = AF_UNSPEC;

		PADDRINFOA queryResult;
		int32_t result = getaddrinfo(Domain.c_str(), "", &hintInfo, &queryResult);
		if (result != 0)
			throw new exception("Invalid Domain");

		for (addrinfo* ptr = queryResult; ptr != NULL; ptr = ptr->ai_next)
		{
			if (ptr->ai_family != AF_INET6)
				continue;

			sockaddr_in6* ipv6 = (struct sockaddr_in6*)ptr->ai_addr;

			return IPAddress(PlatformNetwork::AddressFamilies::InterNetworkV6, ipv6->sin6_addr.u.Byte);
		}

		sockaddr_in* ipv4 = (struct sockaddr_in*)queryResult->ai_addr;

		return IPAddress(PlatformNetwork::AddressFamilies::InterNetwork, ipv4->sin_addr.S_un.S_addr);



		//printf("\tFlags: 0x%x\n", ptr->ai_flags);
		//printf("\tFamily: ");
		//switch (ptr->ai_family) {
		//case AF_UNSPEC:
		//	printf("Unspecified\n");
		//	break;
		//case AF_INET:
		//	printf("AF_INET (IPv4)\n");
		//	sockaddr_in* sockaddr_ipv4 = (struct sockaddr_in*) ptr->ai_addr;
		//	//printf("\tIPv4 address %s\n", inet_ntop(ptr->ai_family, sockaddr_ipv4->sin_addr));
		//	break;
		//case AF_INET6:
		//	printf("AF_INET6 (IPv6)\n");
		//	// the InetNtop function is available on Windows Vista and later
		//	// sockaddr_ipv6 = (struct sockaddr_in6 *) ptr->ai_addr;
		//	// printf("\tIPv6 address %s\n",
		//	//    InetNtop(AF_INET6, &sockaddr_ipv6->sin6_addr, ipstringbuffer, 46) );

		//	// We use WSAAddressToString since it is supported on Windows XP and later
		//	LPSOCKADDR sockaddr_ip = (LPSOCKADDR)ptr->ai_addr;
		//	// The buffer length is changed by each call to WSAAddresstoString
		//	// So we need to set it for each iteration through the loop for safety
		//	DWORD ipbufferlength = 46;
		//	wchar_t ipstringbuffer[46];

		//	INT iRetval = WSAAddressToStringW(sockaddr_ip, (DWORD)ptr->ai_addrlen, NULL,
		//		ipstringbuffer, &ipbufferlength);
		//	if (iRetval)
		//		printf("WSAAddressToString failed with %u\n", WSAGetLastError());
		//	else
		//		printf("\tIPv6 address %s\n", ipstringbuffer);
		//	break;
		//case AF_NETBIOS:
		//	printf("AF_NETBIOS (NetBIOS)\n");
		//	break;
		//default:
		//	printf("Other %ld\n", ptr->ai_family);
		//	break;
		//}
		//printf("\tSocket type: ");
		//switch (ptr->ai_socktype) {
		//case 0:
		//	printf("Unspecified\n");
		//	break;
		//case SOCK_STREAM:
		//	printf("SOCK_STREAM (stream)\n");
		//	break;
		//case SOCK_DGRAM:
		//	printf("SOCK_DGRAM (datagram) \n");
		//	break;
		//case SOCK_RAW:
		//	printf("SOCK_RAW (raw) \n");
		//	break;
		//case SOCK_RDM:
		//	printf("SOCK_RDM (reliable message datagram)\n");
		//	break;
		//case SOCK_SEQPACKET:
		//	printf("SOCK_SEQPACKET (pseudo-stream packet)\n");
		//	break;
		//default:
		//	printf("Other %ld\n", ptr->ai_socktype);
		//	break;
		//}
		//printf("\tProtocol: ");
		//switch (ptr->ai_protocol) {
		//case 0:
		//	printf("Unspecified\n");
		//	break;
		//case IPPROTO_TCP:
		//	printf("IPPROTO_TCP (TCP)\n");
		//	break;
		//case IPPROTO_UDP:
		//	printf("IPPROTO_UDP (UDP) \n");
		//	break;
		//default:
		//	printf("Other %ld\n", ptr->ai_protocol);
		//	break;
		//}
		//printf("\tLength of this sockaddr: %d\n", ptr->ai_addrlen);
		//printf("\tCanonical name: %s\n", ptr->ai_canonname);
	}

	IPAddress SocketUtilities::MapIPv4ToIPv6(IPAddress IP)
	{
		if (IP.GetAddressFamily() != PlatformNetwork::AddressFamilies::InterNetwork)
			throw new exception("IP must be v4");

		return ResolveDomain("::ffff:" + IPAddressToString(IP));
	}

	std::string SocketUtilities::IPAddressToString(const IPAddress& IP)
	{
		if (IP.GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
		{
			char buffer[INET_ADDRSTRLEN];

			uint64_t ipv4 = IP.GetIPv4Address();

			return inet_ntop(AF_INET, &ipv4, buffer, INET_ADDRSTRLEN);
		}

		char buffer[INET6_ADDRSTRLEN];

		const in6_addr* ipv6 = reinterpret_cast<const in6_addr*>(IP.GetIPv6Address());

		return inet_ntop(AF_INET6, &ipv6, buffer, INET6_ADDRSTRLEN);
	}
}