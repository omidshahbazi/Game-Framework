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
		//Socket.Blocking = Value;
	}

	void SocketUtilities::SetReceiveBufferSize(Socket Socket, uint32_t Value)
	{
		//Socket.ReceiveBufferSize = (int)Value;
	}

	void SocketUtilities::SetSendBufferSize(Socket Socket, uint32_t Value)
	{
		//Socket.SendBufferSize = (int)Value;
	}

	void SocketUtilities::SetReceiveTimeout(Socket Socket, uint32_t Value)
	{
		//Socket.ReceiveTimeout = (int)Value;
	}

	void SocketUtilities::SetSendTimeout(Socket Socket, uint32_t Value)
	{
		//Socket.SendTimeout = (int)Value;
	}

	void SocketUtilities::SetTimeToLive(Socket Socket, uint16_t Value)
	{
		//Socket.Ttl = (short)Value;
	}

	void SocketUtilities::SetIPv6OnlyEnabled(Socket Socket, bool Value)
	{
		//Socket.SetSocketOption(SocketOptionLevel.IPv6, IPV6_ONLY_OPTION, Value);
	}

	void SocketUtilities::SetChecksumEnabled(Socket Socket, bool Value)
	{
		//Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoChecksum, (!Value ? 0 : 1));
	}

	void SocketUtilities::SetBSDUrgentEnabled(Socket Socket, bool Value)
	{
		//Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BsdUrgent, Value);
	}

	// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
	// will apply a bad effect on the send/receive protocol under TCP connection
	// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
	// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
	void SocketUtilities::SetNagleAlgorithmEnabled(Socket Socket, bool Value)
	{
		//Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, !Value);
	}

	bool SocketUtilities::IsSocketReady(Socket Socket)
	{
		//return !(Socket.Poll(10, SelectMode.SelectRead) && Socket.Available == 0);
		return false;
	}

	IPAddress SocketUtilities::ResolveDomain(const string& Domain)
	{
		ADDRINFO hintInfo;
		ZeroMemory(&hintInfo, sizeof(ADDRINFO));
		hintInfo.ai_flags = 0;
		hintInfo.ai_family = AF_UNSPEC;

		PADDRINFOA queryResult;
		int32_t host = getaddrinfo(Domain.c_str(), "", &hintInfo, &queryResult);

		PADDRINFOA result = queryResult;

		for (addrinfo* ptr = queryResult; ptr != NULL; ptr = ptr->ai_next)
		{
			if (ptr->ai_family != AF_INET6)
				continue;

			result = ptr;
			break;
		}

		sockaddr_in* sockaddr_ipv4 = (struct sockaddr_in*)result->ai_addr;

		sockaddr_in6* sockaddr_ipv6 = (struct sockaddr_in6*)result->ai_addr;

		return IPAddress(PlatformNetwork::AddressFamilies::InterNetwork, 0);


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
		//if (IP.AddressFamily != AddressFamily.InterNetwork)
		//	throw new ArgumentException("IP must be v4");

		//return IPAddress.Parse("::ffff:" + IP.ToString());

		return IPAddress(PlatformNetwork::AddressFamilies::InterNetwork, 0);
	}
}