// Copyright 2019. All Rights Reserved.
#ifdef WINDOWS
#include "..\Include\PlatformNetwork.h"
#include <Utilities\BitwiseHelper.h>
#include <winsock2.h>
#include <ws2ipdef.h>
#include <ws2tcpip.h>
#include <wdmguid.h>
#include <iphlpapi.h>
#include <icmpapi.h>
#include <icmpapi.h>

#pragma comment(lib, "wsock32.lib")
#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "iphlpapi.lib")


using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
#define BUILD_SOCKET_ADDRESS(AddressFamily, Address, Port) \
		sockaddr* address = nullptr; \
		int32_t addressSize = 0; \
		\
		sockaddr_in addressV4; \
		sockaddr_in6 addressV6; \
		\
		if (AddressFamily == AddressFamilies::InterNetwork) \
		{ \
			addressV4.sin_family = GetAddressFamiliy(AddressFamily); \
			addressV4.sin_port = htons(Port); \
			\
			inet_pton(addressV4.sin_family, Address, reinterpret_cast<void*>(&addressV4.sin_addr)); \
			\
			address = reinterpret_cast<sockaddr*>(&addressV4); \
			addressSize = sizeof(sockaddr_in); \
		} \
		else \
		{ \
			addressV6.sin6_family = GetAddressFamiliy(AddressFamily); \
			addressV6.sin6_port = htons(Port); \
			addressV6.sin6_scope_id = 0; \
			\
			inet_pton(addressV6.sin6_family, Address, reinterpret_cast<void*>(&addressV6.sin6_addr)); \
			\
			address = reinterpret_cast<sockaddr*>(&addressV6); \
			addressSize = sizeof(sockaddr_in6); \
		} \

	std::string AddressToString(PlatformNetwork::AddressFamilies Family, const void* Address)
	{
		char buffer[INET6_ADDRSTRLEN];

		return inet_ntop((Family == PlatformNetwork::AddressFamilies::InterNetwork ? AF_INET : AF_INET6), Address, buffer, (Family == PlatformNetwork::AddressFamilies::InterNetwork ? INET_ADDRSTRLEN : INET6_ADDRSTRLEN));
	}

	int32_t GetAddressFamiliy(PlatformNetwork::AddressFamilies Family)
	{
		switch (Family)
		{
		case PlatformNetwork::AddressFamilies::InterNetwork:
			return AF_INET;

		case PlatformNetwork::AddressFamilies::InterNetworkV6:
			return AF_INET6;
		}

		return AF_UNSPEC;
	}

	int32_t GetType(PlatformNetwork::Types Type)
	{
		switch (Type)
		{
		case PlatformNetwork::Types::Stream:
			return SOCK_STREAM;

		case PlatformNetwork::Types::Datagram:
			return SOCK_DGRAM;

		case PlatformNetwork::Types::RawProtocol:
			return SOCK_RAW;

		case PlatformNetwork::Types::ReliablyDeliverdMessage:
			return SOCK_RDM;

		case PlatformNetwork::Types::SequenedPacketStream:
			return SOCK_SEQPACKET;
		}

		return 0;
	}

	int32_t GetIPProtocol(PlatformNetwork::IPProtocols IPProtocol, PlatformNetwork::AddressFamilies Family)
	{
		switch (IPProtocol)
		{
		case PlatformNetwork::IPProtocols::TCP:
			return IPPROTO_TCP;

		case PlatformNetwork::IPProtocols::UDP:
			return IPPROTO_UDP;

		case PlatformNetwork::IPProtocols::ICMP:
			return (Family == PlatformNetwork::AddressFamilies::InterNetwork ? IPPROTO_ICMP : IPPROTO_ICMPV6);
		}

		return IPPROTO_MAX;
	}

	int32_t GetSendFlags(PlatformNetwork::SendModes Mode)
	{
		int32_t flags = 0;

		if (BitwiseHelper::IsEnabled((int32_t)Mode, (int32_t)PlatformNetwork::SendModes::DontRoute))
			flags |= MSG_DONTROUTE;

		if (BitwiseHelper::IsEnabled((int32_t)Mode, (int32_t)PlatformNetwork::SendModes::OutOfBand))
			flags |= MSG_OOB;

		return flags;
	}

	int32_t GetReceiveFlags(PlatformNetwork::ReceiveModes Mode)
	{
		int32_t flags = 0;

		if (BitwiseHelper::IsEnabled((int32_t)Mode, (int32_t)PlatformNetwork::ReceiveModes::DontRoute))
			flags |= MSG_DONTROUTE;

		if (BitwiseHelper::IsEnabled((int32_t)Mode, (int32_t)PlatformNetwork::ReceiveModes::OutOfBand))
			flags |= MSG_OOB;

		if (BitwiseHelper::IsEnabled((int32_t)Mode, (int32_t)PlatformNetwork::ReceiveModes::Peek))
			flags |= MSG_PEEK;

		return flags;
	}

	int32_t GetSelectMode(PlatformNetwork::SelectModes Mode)
	{
		switch (Mode)
		{
		case PlatformNetwork::SelectModes::SelectRead:
			return POLLRDNORM;

		case PlatformNetwork::SelectModes::SelectWrite:
			return POLLWRNORM;

		case PlatformNetwork::SelectModes::SelectError:
			return POLLERR;
		}

		return 0;
	}

	PlatformNetwork::Errors GetError(int32_t ErrorCode)
	{
		switch (ErrorCode)
		{
		case WSABASEERR: return PlatformNetwork::Errors::BaseError;
		case WSAEINTR: return PlatformNetwork::Errors::Interrupted;
		case WSAEBADF: return PlatformNetwork::Errors::BadFile;
		case WSAEACCES: return PlatformNetwork::Errors::AccessDenied;
		case WSAEFAULT: return PlatformNetwork::Errors::InvalidPointer;
		case WSAEINVAL: return PlatformNetwork::Errors::InvalidArguments;
		case WSAEMFILE: return PlatformNetwork::Errors::TooManySockets;
		case WSAEWOULDBLOCK: return PlatformNetwork::Errors::WouldBlock;
		case WSAEINPROGRESS: return PlatformNetwork::Errors::BlockingInProgress;
		case WSAEALREADY: return PlatformNetwork::Errors::NonBlockingInProgress;
		case WSAENOTSOCK: return PlatformNetwork::Errors::NoSocket;
		case WSAEDESTADDRREQ: return PlatformNetwork::Errors::DestinationAddressRequired;
		case WSAEMSGSIZE: return PlatformNetwork::Errors::LargePacketSize;
		case WSAEPROTOTYPE: return PlatformNetwork::Errors::MismatchProtocolType;
		case WSAENOPROTOOPT: return PlatformNetwork::Errors::NoProtocolOperation;
		case WSAEPROTONOSUPPORT: return PlatformNetwork::Errors::ProtocolNotSupported;
		case WSAESOCKTNOSUPPORT: return PlatformNetwork::Errors::SocketNotSupported;
		case WSAEOPNOTSUPP: return PlatformNetwork::Errors::OperationNotSupported;
		case WSAEPFNOSUPPORT: return PlatformNetwork::Errors::ProtocolFamilyNotSupported;
		case WSAEAFNOSUPPORT: return PlatformNetwork::Errors::AddressFamilyNotSupported;
		case WSAEADDRINUSE: return PlatformNetwork::Errors::AddressInUse;
		case WSAEADDRNOTAVAIL: return PlatformNetwork::Errors::AddressNotValid;
		case WSAENETDOWN: return PlatformNetwork::Errors::NetworkDown;
		case WSAENETUNREACH: return PlatformNetwork::Errors::NetworkUnreachable;
		case WSAENETRESET: return PlatformNetwork::Errors::NetworkReset;
		case WSAECONNABORTED: return PlatformNetwork::Errors::ConnectionAborted;
		case WSAECONNRESET: return PlatformNetwork::Errors::ConnectionReset;
		case WSAENOBUFS: return PlatformNetwork::Errors::NoBuffer;
		case WSAEISCONN: return PlatformNetwork::Errors::IsConnected;
		case WSAENOTCONN: return PlatformNetwork::Errors::NotConnected;
		case WSAESHUTDOWN: return PlatformNetwork::Errors::Shutdown;
		case WSAETOOMANYREFS: return PlatformNetwork::Errors::TooManyReferences;
		case WSAETIMEDOUT: return PlatformNetwork::Errors::Timeout;
		case WSAECONNREFUSED: return PlatformNetwork::Errors::ConnectionRefused;
		case WSAELOOP: return PlatformNetwork::Errors::Loop;
		case WSAENAMETOOLONG: return PlatformNetwork::Errors::NameTooLong;
		case WSAEHOSTDOWN: return PlatformNetwork::Errors::HostDown;
		case WSAEHOSTUNREACH: return PlatformNetwork::Errors::HostUnreachable;
		case WSAENOTEMPTY: return PlatformNetwork::Errors::NotEmpty;
		case WSAEPROCLIM: return PlatformNetwork::Errors::ProcessLimit;
		case WSAEUSERS: return PlatformNetwork::Errors::OutOfUsers;
		case WSAEDQUOT: return PlatformNetwork::Errors::OutOfDisk;
		case WSAESTALE: return PlatformNetwork::Errors::HandleNotExists;
		case WSAEREMOTE: return PlatformNetwork::Errors::ItemNotExists;
		case WSASYSNOTREADY: return PlatformNetwork::Errors::SystemNotReady;
		case WSAVERNOTSUPPORTED: return PlatformNetwork::Errors::VersionNotSupported;
		case WSANOTINITIALISED: return PlatformNetwork::Errors::NotInitialized;
		case WSAEDISCON: return PlatformNetwork::Errors::Disconnected;
		case WSAEINVALIDPROCTABLE: return PlatformNetwork::Errors::InvalidProcedureTable;
		case WSAEINVALIDPROVIDER: return PlatformNetwork::Errors::InvalidProvider;
		case WSAEPROVIDERFAILEDINIT: return PlatformNetwork::Errors::ProviderFailedToInitialize;
		case WSASYSCALLFAILURE: return PlatformNetwork::Errors::SystemCallFailure;
		case WSASERVICE_NOT_FOUND: return PlatformNetwork::Errors::ServiceNotFound;
		case WSATYPE_NOT_FOUND: return PlatformNetwork::Errors::TypeNotFound;
		case WSA_E_NO_MORE: return PlatformNetwork::Errors::NoMoreResult2;
		case WSA_E_CANCELLED: return PlatformNetwork::Errors::Canceled2;
		case WSAEREFUSED: return PlatformNetwork::Errors::Refused;
		case WSAHOST_NOT_FOUND: return PlatformNetwork::Errors::HostNotFound;
		case WSATRY_AGAIN: return PlatformNetwork::Errors::TryAgain;
		case WSANO_RECOVERY: return PlatformNetwork::Errors::NoRecoverable;
		case WSANO_DATA: return PlatformNetwork::Errors::NoData;
		case WSA_QOS_RECEIVERS: return PlatformNetwork::Errors::QOSReceivers;
		case WSA_QOS_NO_SENDERS: return PlatformNetwork::Errors::QOSNoSenders;
		case WSA_QOS_NO_RECEIVERS: return PlatformNetwork::Errors::QOSNoReceiver;
		case WSA_QOS_REQUEST_CONFIRMED: return PlatformNetwork::Errors::QOSRequestConfirmed;
		case WSA_QOS_ADMISSION_FAILURE: return PlatformNetwork::Errors::AdmissionFailure;
		case WSA_QOS_POLICY_FAILURE: return PlatformNetwork::Errors::QOSPolicyFailure;
		case WSA_QOS_BAD_STYLE: return PlatformNetwork::Errors::QOSBadStyle;
		case WSA_QOS_BAD_OBJECT: return PlatformNetwork::Errors::QOSBadObject;
		case WSA_QOS_TRAFFIC_CTRL_ERROR: return PlatformNetwork::Errors::QOSTrafficControlError;
		case WSA_QOS_GENERIC_ERROR: return PlatformNetwork::Errors::QOSGenericError;
		case WSA_QOS_ESERVICETYPE: return PlatformNetwork::Errors::QOSServiceType;
		case WSA_QOS_EFLOWSPEC: return PlatformNetwork::Errors::QOSFlowSpecific;
		case WSA_QOS_EPROVSPECBUF: return PlatformNetwork::Errors::QOSProviderSpecific;
		case WSA_QOS_EFILTERSTYLE: return PlatformNetwork::Errors::QOSFilterStyle;
		case WSA_QOS_EFILTERTYPE: return PlatformNetwork::Errors::QOSFilterType;
		case WSA_QOS_EFILTERCOUNT: return PlatformNetwork::Errors::QOSFilterCount;
		case WSA_QOS_EOBJLENGTH: return PlatformNetwork::Errors::QOSObjectLength;
		case WSA_QOS_EFLOWCOUNT: return PlatformNetwork::Errors::QOSLowCount;
		case WSA_QOS_EUNKOWNPSOBJ: return PlatformNetwork::Errors::QOSUnknownProviderSpecific;
		case WSA_QOS_EPOLICYOBJ: return PlatformNetwork::Errors::QOSPolicyObject;
		case WSA_QOS_EFLOWDESC: return PlatformNetwork::Errors::QOSFlowDescriptor;
		case WSA_QOS_EPSFLOWSPEC: return PlatformNetwork::Errors::QOSInconsistentFlowSpecific;
		case WSA_QOS_EPSFILTERSPEC: return PlatformNetwork::Errors::QOSInconsistentFilterSpecific;
		case WSA_QOS_ESDMODEOBJ: return PlatformNetwork::Errors::QOSShapeDiscardMode;
		case WSA_QOS_ESHAPERATEOBJ: return PlatformNetwork::Errors::QOSShapingRateObject;
		case WSA_QOS_RESERVED_PETYPE: return PlatformNetwork::Errors::QOSReceivedProviderType;
		case WSA_SECURE_HOST_NOT_FOUND: return PlatformNetwork::Errors::QOSSecureHostNotFound;
		case WSA_IPSEC_NAME_POLICY_ERROR: return PlatformNetwork::Errors::IPSecPolicy;
		case ERROR_INSUFFICIENT_BUFFER:return PlatformNetwork::Errors::InsufficientBuffer;
		case ERROR_INVALID_PARAMETER: return PlatformNetwork::Errors::InvalidParameters;
		case ERROR_IO_PENDING: return PlatformNetwork::Errors::IOPending;
		case ERROR_NOT_ENOUGH_MEMORY: return PlatformNetwork::Errors::NotEnoughMemory;
		case ERROR_NOT_SUPPORTED: return PlatformNetwork::Errors::NotSupported;
		}

		return PlatformNetwork::Errors::NoError;
	}

	int32_t GetShutdownHow(PlatformNetwork::ShutdownHows How)
	{
		switch (How)
		{
		case PlatformNetwork::ShutdownHows::Receive:
			return SD_RECEIVE;

		case PlatformNetwork::ShutdownHows::Send:
			return SD_SEND;

		case PlatformNetwork::ShutdownHows::Both:
			return SD_BOTH;
		}

		return SD_BOTH;
	}

	int32_t GetOptionLevel(PlatformNetwork::OptionLevels Option)
	{
		switch (Option)
		{
		case PlatformNetwork::OptionLevels::IP:
			return IPPROTO_IP;

		case PlatformNetwork::OptionLevels::TCP:
			return IPPROTO_TCP;

		case PlatformNetwork::OptionLevels::UDP:
			return IPPROTO_UDP;

		case PlatformNetwork::OptionLevels::IPV6:
			return IPPROTO_IPV6;

		case PlatformNetwork::OptionLevels::Socket:
			return SOL_SOCKET;
		}

		return IPPROTO_IP;
	}

	int32_t GetOption(PlatformNetwork::Options Option)
	{
		switch (Option)
		{
		case PlatformNetwork::Options::Debug:
			return SO_DEBUG;

		case PlatformNetwork::Options::AcceptConnection:
			return SO_ACCEPTCONN;

		case PlatformNetwork::Options::ReuseAddress:
			return SO_REUSEADDR;

		case PlatformNetwork::Options::KeepAlive:
			return SO_KEEPALIVE;

		case PlatformNetwork::Options::DontRoute:
			return SO_DONTROUTE;

		case PlatformNetwork::Options::Broadcast:
			return SO_BROADCAST;

		case PlatformNetwork::Options::UseLoopback:
			return SO_USELOOPBACK;

		case PlatformNetwork::Options::Linger:
			return SO_LINGER;

		case PlatformNetwork::Options::DontLinger:
			return SO_DONTLINGER;

		case PlatformNetwork::Options::SendBuffer:
			return SO_SNDBUF;

		case PlatformNetwork::Options::ReceiveBuffer:
			return SO_RCVBUF;

		case PlatformNetwork::Options::SendTimeout:
			return SO_SNDTIMEO;

		case PlatformNetwork::Options::ReceiveTimeout:
			return SO_RCVTIMEO;

		case PlatformNetwork::Options::GroupID:
			return SO_GROUP_ID;

		case PlatformNetwork::Options::GroupPriority:
			return SO_GROUP_PRIORITY;

		case PlatformNetwork::Options::MaxMessageSize:
			return SO_MAX_MSG_SIZE;

		case PlatformNetwork::Options::ConditionalAccept:
			return SO_CONDITIONAL_ACCEPT;

		case PlatformNetwork::Options::PauseAccept:
			return SO_PAUSE_ACCEPT;

		case PlatformNetwork::Options::RandomizePort:
			return SO_RANDOMIZE_PORT;

		case PlatformNetwork::Options::PortScalability:
			return SO_PORT_SCALABILITY;

		case PlatformNetwork::Options::ReuseUnicastPort:
			return SO_REUSE_UNICASTPORT;

		case PlatformNetwork::Options::ReuseMulticastPort:
			return SO_REUSE_MULTICASTPORT;

		case PlatformNetwork::Options::NoDelay:
			return TCP_NODELAY;

		case PlatformNetwork::Options::TimeToLive:
			return IP_TTL;

		case PlatformNetwork::Options::IPv6Only:
			return IPV6_V6ONLY;

		case PlatformNetwork::Options::Checksum:
			return IPV6_CHECKSUM;
		}
	}

	PlatformNetwork::PingStatus GetPingStatus(int32_t ErrorCode)
	{
		switch (ErrorCode)
		{
		case IP_SUCCESS: return PlatformNetwork::PingStatus::Success;
		case IP_DEST_NET_UNREACHABLE: return PlatformNetwork::PingStatus::DestinationNetworkUnreachable;
		case IP_DEST_HOST_UNREACHABLE: return PlatformNetwork::PingStatus::DestinationHostUnreachable;
		case IP_DEST_PROT_UNREACHABLE: return PlatformNetwork::PingStatus::DestinationProtocolUnreachable;
		case IP_DEST_PORT_UNREACHABLE: return PlatformNetwork::PingStatus::DestinationPortUnreachable;
		case IP_NO_RESOURCES: return PlatformNetwork::PingStatus::DestinationPortUnreachable;
		case IP_BAD_OPTION: return PlatformNetwork::PingStatus::BadOption;
		case IP_HW_ERROR: return PlatformNetwork::PingStatus::HardwareError;
		case IP_PACKET_TOO_BIG: return PlatformNetwork::PingStatus::PacketTooBig;
		case IP_REQ_TIMED_OUT: return PlatformNetwork::PingStatus::TimedOut;
		case IP_BAD_ROUTE: return PlatformNetwork::PingStatus::BadRoute;
		case IP_TTL_EXPIRED_TRANSIT: return PlatformNetwork::PingStatus::TTLExpired;
		case IP_TTL_EXPIRED_REASSEM: return PlatformNetwork::PingStatus::TTLReassemblyTimeExceeded;
		case IP_PARAM_PROBLEM: return PlatformNetwork::PingStatus::ParameterProblem;
		case IP_SOURCE_QUENCH: return PlatformNetwork::PingStatus::SourceQuench;
		case IP_BAD_DESTINATION: return PlatformNetwork::PingStatus::BadDestination;
		case IP_DEST_UNREACHABLE: return PlatformNetwork::PingStatus::DestinationUnreachable;
		case IP_TIME_EXCEEDED: return PlatformNetwork::PingStatus::TimeExceeded;
		case IP_BAD_HEADER: return PlatformNetwork::PingStatus::BadHeader;
		case IP_UNRECOGNIZED_NEXT_HEADER: return PlatformNetwork::PingStatus::UnrecognizedNextHeader;
		case IP_ICMP_ERROR: return PlatformNetwork::PingStatus::ICMPError;
		case IP_DEST_SCOPE_MISMATCH: return PlatformNetwork::PingStatus::DestinationScopeMismatch;
		}

		return PlatformNetwork::PingStatus::Unknown;
	}

	void PlatformNetwork::Initialize(void)
	{
		WSADATA data;
		if (WSAStartup(MAKEWORD(2, 2), &data) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::Shutdown(void)
	{
		if (WSACleanup() == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	PlatformNetwork::Handle PlatformNetwork::Create(AddressFamilies AddressFamily, Types Type, IPProtocols IPProtocol)
	{
		return (Handle)socket(GetAddressFamiliy(AddressFamily), GetType(Type), GetIPProtocol(IPProtocol, AddressFamily));
	}

	void PlatformNetwork::Shutdown(Handle Handle, ShutdownHows How)
	{
		if (shutdown(Handle, GetShutdownHow(How)) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::Close(Handle Handle)
	{
		if (closesocket(Handle) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::SetSocketOption(Handle Handle, OptionLevels Level, Options Option, bool Enabled)
	{
		//if (setsockopt(Handle, GetOptionLevel(Level), GetOption(Option), reinterpret_cast<char*>(&Enabled), sizeof(bool)) == SOCKET_ERROR)
		//	throw SocketException(GetLastError());

		SetSocketOption(Handle, Level, Option, (Enabled ? 1 : 0));
	}

	void PlatformNetwork::SetSocketOption(Handle Handle, OptionLevels Level, Options Option, int32_t Value)
	{
		if (setsockopt(Handle, GetOptionLevel(Level), GetOption(Option), reinterpret_cast<char*>(&Value), sizeof(int32_t)) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::SetBlocking(Handle Handle, bool Enabled)
	{
		DWORD enabled = (Enabled ? 0 : 1);

		if (ioctlsocket(Handle, FIONBIO, &enabled) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::Bind(Handle Handle, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port)
	{
		BUILD_SOCKET_ADDRESS(AddressFamily, Address.c_str(), Port);

		if (bind(Handle, address, addressSize) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	void PlatformNetwork::Listen(Handle Handle, uint32_t MaxConnections)
	{
		if (listen(Handle, MaxConnections) == SOCKET_ERROR)
			throw SocketException(GetLastError());
	}

	bool PlatformNetwork::Accept(Handle ListenerHandle, Handle& AcceptedHandle, AddressFamilies& AddressFamily, std::string& Address, uint16_t& Port)
	{
		sockaddr* address = nullptr;
		int32_t size = 0;

		sockaddr_in ipv4;
		sockaddr_in6 ipv6;

		if (AddressFamily == PlatformNetwork::AddressFamilies::InterNetwork)
		{
			address = reinterpret_cast<sockaddr*>(&ipv4);
			size = sizeof(sockaddr_in);
		}
		else
		{
			address = reinterpret_cast<sockaddr*>(&ipv6);
			size = sizeof(sockaddr_in6);
		}

		int32_t result = accept(ListenerHandle, address, &size);

		if (result == INVALID_SOCKET)
		{
			Errors error = GetLastError();

			if (error == Errors::WouldBlock)
				return false;

			throw SocketException(GetLastError());
		}

		AcceptedHandle = result;

		if (size == sizeof(sockaddr_in))
		{
			AddressFamily = PlatformNetwork::AddressFamilies::InterNetwork;
			Address = AddressToString(AddressFamily, &ipv4.sin_addr);
			Port = ntohs(ipv4.sin_port);
		}
		else if (size == sizeof(sockaddr_in6))
		{
			AddressFamily = PlatformNetwork::AddressFamilies::InterNetworkV6;
			Address = AddressToString(AddressFamily, &ipv6.sin6_addr);
			Port = ntohs(ipv6.sin6_port);
		}
		else
			throw SocketException(Errors::InvalidArguments);

		return true;
	}

	bool PlatformNetwork::Connect(Handle Handle, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port)
	{
		BUILD_SOCKET_ADDRESS(AddressFamily, Address.c_str(), Port);

		if (connect(Handle, address, addressSize) == SOCKET_ERROR)
			throw SocketException(GetLastError());

		return true;
	}

	uint32_t PlatformNetwork::Send(Handle Handle, const std::byte* Buffer, uint32_t Length, SendModes Mode)
	{
		int32_t result = send(Handle, reinterpret_cast<const char*>(Buffer), Length, GetSendFlags(Mode));

		if (result == SOCKET_ERROR)
			throw SocketException(GetLastError());

		return result;
	}

	uint32_t PlatformNetwork::SendTo(Handle Handle, const std::byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port, SendModes Mode)
	{
		BUILD_SOCKET_ADDRESS(AddressFamily, Address.c_str(), Port);

		int32_t result = sendto(Handle, reinterpret_cast<const char*>(Buffer), Length, GetSendFlags(Mode), address, addressSize);

		if (result == SOCKET_ERROR)
			throw SocketException(GetLastError());

		return result;
	}

	bool PlatformNetwork::Poll(Handle Handle, uint32_t Timeout, SelectModes Mode)
	{
		const uint32_t COUNT = 1;

		int32_t mode = GetSelectMode(Mode);

		WSAPOLLFD fd;
		fd.fd = Handle;
		fd.events = mode;

		if (WSAPoll(&fd, COUNT, Timeout) != COUNT)
			return false;

		if (fd.revents & POLLERR)
			return false;

		if (fd.revents & POLLHUP)
			return false;

		if (fd.revents & POLLNVAL)
			return false;

		return fd.revents & mode;
	}

	bool PlatformNetwork::Select(Handle Handle, SelectModes Mode, uint32_t Timeout)
	{
		fd_set set;
		FD_ZERO(&set);
		FD_SET(Handle, &set);

		timeval tv;
		tv.tv_sec = Timeout / 1000;

		int32_t result = 0;
		if (Mode == SelectModes::SelectRead)
			result = select(Handle + 1, &set, nullptr, nullptr, &tv);
		else if (Mode == SelectModes::SelectWrite)
			result = select(Handle + 1, nullptr, &set, nullptr, &tv);
		else if (Mode == SelectModes::SelectError)
			result = select(Handle + 1, nullptr, nullptr, &set, &tv);

		if (result == 0)
			throw SocketException(PlatformNetwork::Errors::Timeout);

		if (result == SOCKET_ERROR)
			throw SocketException(GetLastError());

		return true;
	}

	uint64_t PlatformNetwork::GetAvailableBytes(Handle Handle)
	{
		u_long availableBytes;

		if (ioctlsocket(Handle, FIONREAD, &availableBytes) == SOCKET_ERROR)
			return 0;

		return availableBytes;
	}

	bool PlatformNetwork::Receive(Handle Handle, std::byte* Buffer, uint32_t& Length, ReceiveModes Mode)
	{
		int32_t result = recv(Handle, reinterpret_cast<char*>(Buffer), Length, GetReceiveFlags(Mode));

		if (result == SOCKET_ERROR)
			throw SocketException(GetLastError());

		Length = result;

		return (Length != 0);
	}

	bool PlatformNetwork::ReceiveFrom(Handle Handle, std::byte* Buffer, uint32_t& Length, AddressFamilies AddressFamily, std::string& Address, uint16_t& Port, ReceiveModes Mode)
	{
		sockaddr* address = nullptr;
		int32_t size = 0;

		sockaddr_in ipv4;
		sockaddr_in6 ipv6;

		if (AddressFamily == PlatformNetwork::AddressFamilies::InterNetwork)
		{
			address = reinterpret_cast<sockaddr*>(&ipv4);
			size = sizeof(sockaddr_in);
		}
		else
		{
			address = reinterpret_cast<sockaddr*>(&ipv6);
			size = sizeof(sockaddr_in6);
		}

		Length = recvfrom(Handle, reinterpret_cast<char*>(Buffer), Length, GetReceiveFlags(Mode), reinterpret_cast<sockaddr*>(address), &size);

		if (Length == SOCKET_ERROR)
			throw SocketException(GetLastError());

		if (Length != 0)
		{
			if (size == sizeof(sockaddr_in))
			{
				Address = AddressToString(PlatformNetwork::AddressFamilies::InterNetwork, &ipv4.sin_addr);
				Port = ntohs(ipv4.sin_port);
			}
			else if (size == sizeof(sockaddr_in6))
			{
				Address = AddressToString(PlatformNetwork::AddressFamilies::InterNetworkV6, &ipv6.sin6_addr);
				Port = ntohs(ipv6.sin6_port);
			}
			else
				throw SocketException(Errors::InvalidArguments);
		}

		return (Length != 0);
	}

	void PlatformNetwork::ResolveDomain(const std::string& Domain, AddressFamilies& AddressFamily, std::string& Address)
	{
		ADDRINFO hintInfo;
		ZeroMemory(&hintInfo, sizeof(ADDRINFO));
		hintInfo.ai_flags = 0;
		hintInfo.ai_family = AF_UNSPEC;

		PADDRINFOA queryResult;
		int32_t result = getaddrinfo(Domain.c_str(), "", &hintInfo, &queryResult);
		if (result != 0)
			throw SocketException("Invalid Domain");

		for (addrinfo* ptr = queryResult; ptr != NULL; ptr = ptr->ai_next)
		{
			if (ptr->ai_family != AF_INET6)
				continue;

			sockaddr_in6* ipv6 = (struct sockaddr_in6*)ptr->ai_addr;

			AddressFamily = PlatformNetwork::AddressFamilies::InterNetworkV6;
			Address = AddressToString(AddressFamily, &ipv6->sin6_addr);

			return;
		}

		sockaddr_in* ipv4 = (struct sockaddr_in*)queryResult->ai_addr;

		AddressFamily = PlatformNetwork::AddressFamilies::InterNetwork;
		Address = AddressToString(AddressFamily, &ipv4->sin_addr);
	}

	PlatformNetwork::Errors PlatformNetwork::GetLastError(void)
	{
		return GetError(WSAGetLastError());
	}

	PlatformNetwork::PingReply PlatformNetwork::Ping(AddressFamilies AddressFamily, const std::string& Address, uint32_t Timeout, std::byte* Buffer, uint32_t BufferLength, const PingOptions& Options)
	{
		BUILD_SOCKET_ADDRESS(AddressFamily, Address.c_str(), 0);

		int32_t payloadSize = BufferLength;
		if (AddressFamily == AddressFamilies::InterNetwork)
			payloadSize -= sizeof(ICMP_ECHO_REPLY);
		else
			payloadSize -= sizeof(ICMPV6_ECHO_REPLY) + sizeof(uint64_t);

		if (payloadSize <= 0)
			throw SocketException(Errors::InsufficientBuffer);

		IP_OPTION_INFORMATION options;
		ZeroMemory(&options, sizeof(IP_OPTION_INFORMATION));
		options.Ttl = Options.TTL;
		if (Options.DontFragment)
			options.Flags |= IP_FLAG_DF;
		if (Options.Reverse)
			options.Flags |= IP_FLAG_REVERSE;

		uint32_t result = 0;
		if (AddressFamily == AddressFamilies::InterNetwork)
		{
			HANDLE handle = IcmpCreateFile();
			if (handle == INVALID_HANDLE_VALUE)
				throw SocketException(GetLastError());

			result = IcmpSendEcho(handle, addressV4.sin_addr.S_un.S_addr, reinterpret_cast<void*>(Buffer), payloadSize, &options, reinterpret_cast<void*>(Buffer), BufferLength, Timeout);

			ICMP_ECHO_REPLY* reply = reinterpret_cast<ICMP_ECHO_REPLY*>(Buffer);

			IcmpCloseHandle(handle);

			return PingReply(GetPingStatus(reply->Status), reply->RoundTripTime);
		}
		else
		{
			HANDLE handle = Icmp6CreateFile();
			if (handle == INVALID_HANDLE_VALUE)
				throw SocketException(GetLastError());

			result = Icmp6SendEcho2(handle, nullptr, nullptr, nullptr, &addressV6, &addressV6, reinterpret_cast<void*>(Buffer), payloadSize, &options, reinterpret_cast<void*>(Buffer), BufferLength, Timeout);

			ICMPV6_ECHO_REPLY* reply = reinterpret_cast<ICMPV6_ECHO_REPLY*>(Buffer);

			IcmpCloseHandle(handle);

			return PingReply(GetPingStatus(reply->Status), reply->RoundTripTime);
		}
	}
}
#endif