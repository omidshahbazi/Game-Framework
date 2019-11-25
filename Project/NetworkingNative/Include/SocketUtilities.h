// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef SOCKET_UTILITIES_H
#define SOCKET_UTILITIES_H

#include "Common.h"
#include "PlatformNetwork.h"
#include <string>

namespace GameFramework::Networking
{
	typedef PlatformNetwork::Handle Socket;

	struct IPAddress
	{
	public:
		IPAddress(PlatformNetwork::AddressFamilies Family, long Address) :
			m_Family(Family),
			m_Address(Address)
		{
		}

		PlatformNetwork::AddressFamilies GetFamily(void) const
		{
			return m_Family;
		}

		long GetAddress(void) const
		{
			return m_Address;
		}

		std::string ToString(void) const
		{
			return "";
		}

	private:
		PlatformNetwork::AddressFamilies m_Family;
		long m_Address;
	};

	static class NETWORKING_API SocketUtilities
	{
	public:
		static Socket CreateSocket(PlatformNetwork::IPProtocols Protocol);

		static void CloseSocket(Socket Socket);

		static void SetBlocking(Socket Socket, bool Value);

		static void SetReceiveBufferSize(Socket Socket, uint32_t Value);

		static void SetSendBufferSize(Socket Socket, uint32_t Value);

		static void SetReceiveTimeout(Socket Socket, uint32_t Value);

		static void SetSendTimeout(Socket Socket, uint32_t Value);

		static void SetTimeToLive(Socket Socket, uint16_t Value);

		static void SetIPv6OnlyEnabled(Socket Socket, bool Value);

		static void SetChecksumEnabled(Socket Socket, bool Value);

		static void SetBSDUrgentEnabled(Socket Socket, bool Value);

		// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
		// will apply a bad effect on the send/receive protocol under TCP connection
		// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
		// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
		static void SetNagleAlgorithmEnabled(Socket Socket, bool Value);

		static bool IsSocketReady(Socket Socket);

		static IPAddress ResolveDomain(const std::string& Domain);

		static IPAddress MapIPv4ToIPv6(IPAddress IP);
	};
}

#endif