// Copyright 2019. All Rights Reserved.
#include "..\Include\SocketUtilities.h"

using namespace std;

namespace GameFramework::Networking
{
	const IPAddress IPAddress::Any(PlatformNetwork::AddressFamilies::InterNetwork, "0.0.0.0");
	const IPAddress IPAddress::AnyV6(PlatformNetwork::AddressFamilies::InterNetworkV6, "::0");

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
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::IPV6, PlatformNetwork::Options::IPv6Only, Value);
	}

	void SocketUtilities::SetChecksumEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::UDP, PlatformNetwork::Options::Checksum, Value);
	}

	// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
	// will apply a bad effect on the send/receive protocol under TCP connection
	// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
	// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
	void SocketUtilities::SetNagleAlgorithmEnabled(Socket Socket, bool Value)
	{
		PlatformNetwork::SetSocketOption(Socket, PlatformNetwork::OptionLevels::TCP, PlatformNetwork::Options::NoDelay, !Value);
	}

	bool SocketUtilities::GetIsReady(Socket Socket)
	{
		return true;

		//return !(PlatformNetwork::Poll(Socket, 10, PlatformNetwork::SelectModes::SelectRead) && GetAvailableBytes(Socket) == 0);

		//byte data[1];
		//uint32_t size = sizeof(data);
		//return PlatformNetwork::Receive(Socket, data, size, PlatformNetwork::ReceiveModes::Peek);
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
		PlatformNetwork::AddressFamilies family = EndPoint.GetAddress().GetAddressFamily();
		std::string ip;
		uint16_t port;

		if (!PlatformNetwork::Accept(ListenerSocket, AcceptedSocket, family, ip, port))
			return false;

		EndPoint.SetAddress(IPAddress(family, ip));
		EndPoint.SetPort(port);

		return true;
	}

	bool SocketUtilities::Connect(Socket Socket, const IPEndPoint& EndPoint)
	{
		try
		{
			return PlatformNetwork::Connect(Socket, EndPoint.GetAddress().GetAddressFamily(), EndPoint.GetAddress().GetIP(), EndPoint.GetPort());
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.GetError() == PlatformNetwork::Errors::WouldBlock)
				return true;

			throw e;
		}
	}

	uint32_t SocketUtilities::Send(Socket Socket, const std::byte* Buffer, uint32_t Length)
	{
		return PlatformNetwork::Send(Socket, Buffer, Length, PlatformNetwork::SendModes::None);
	}

	uint32_t SocketUtilities::SendTo(Socket Socket, const IPEndPoint& EndPoint, const std::byte* Buffer, uint32_t Length)
	{
		return PlatformNetwork::SendTo(Socket, Buffer, Length, EndPoint.GetAddress().GetAddressFamily(), EndPoint.GetAddress().GetIP(), EndPoint.GetPort(), PlatformNetwork::SendModes::None);
	}

	bool SocketUtilities::Select(Socket Socket, PlatformNetwork::SelectModes Mode, uint32_t Timeout)
	{
		return PlatformNetwork::Select(Socket, Mode, Timeout);
	}

	uint64_t SocketUtilities::GetAvailableBytes(Socket Socket)
	{
		return PlatformNetwork::GetAvailableBytes(Socket);
	}

	bool SocketUtilities::Receive(Socket Socket, std::byte* Buffer, uint32_t Index, uint32_t& Length)
	{
		return PlatformNetwork::Receive(Socket, Buffer, Index, Length, PlatformNetwork::ReceiveModes::None);
	}

	bool SocketUtilities::ReceiveFrom(Socket Socket, std::byte* Buffer, uint32_t& Length, IPEndPoint& EndPoint)
	{
		std::string address;
		uint16_t port;

		bool result = PlatformNetwork::ReceiveFrom(Socket, Buffer, Length, EndPoint.GetAddress().GetAddressFamily(), address, port, PlatformNetwork::ReceiveModes::None);

		if (result)
			EndPoint = IPEndPoint(IPAddress(EndPoint.GetAddress().GetAddressFamily(), address), port);

		return result;
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
			throw PlatformNetwork::SocketException("IP must be v4");

		return ResolveDomain("::ffff:" + IP.GetIP());
	}

	uint32_t SocketUtilities::FindOptimumMTU(IPAddress IP, uint32_t Timeout, uint32_t MaxMTU)
	{
		IP = ResolveDomain(IP.GetIP());

		std::vector<std::byte> buffer(MaxMTU);
		for (uint32_t i = 0; i < MaxMTU; ++i)
			buffer.push_back((std::byte)(i % 255));

		PlatformNetwork::PingOptions options;
		options.DontFragment = true;

		PlatformNetwork::PingReply reply;
		uint32_t size = MaxMTU;
		do
		{
			reply = PlatformNetwork::Ping(IP.GetAddressFamily(), IP.GetIP(), Timeout, const_cast<std::byte*>(buffer.data()), size, options);

			if (reply.Status == PlatformNetwork::PingStatus::Success)
				break;

			--size;

			if (reply.Status == PlatformNetwork::PingStatus::PacketTooBig)
				continue;

			throw PlatformNetwork::SocketException("Finding optimum MTU failed with error");

		} while (size != 0);

		return size;
	}

	void SocketUtilities::OpenDynamicTCPPorts(uint16_t From, uint16_t Count)
	{
		//netsh int ipv4 set dynamicport tcp start=1500 num=63000
		//netsh int ipv4 show dynamicport tcp

		string str = "netsh int ipv4 set dynamicport tcp start=" + to_string(From) + " num=" + to_string(Count);

		system(str.c_str());
	}
}