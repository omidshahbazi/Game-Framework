// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef SOCKET_UTILITIES_H
#define SOCKET_UTILITIES_H

#include "Common.h"
#include "PlatformNetwork.h"

namespace GameFramework::Networking
{
	static class NETWORKING_API SocketUtilities
	{
	public:
		typedef PlatformNetwork::Handle Socket;

	public:
		static Socket CreateSocket(PlatformNetwork::IPProtocols Protocol)
		{
			PlatformNetwork::Types type = (Protocol == PlatformNetwork::IPProtocols::TCP ? PlatformNetwork::Types::Stream : PlatformNetwork::Types::Datagram);

			return PlatformNetwork::Create(PlatformNetwork::AddressFamilies::InterNetworkV6, type, Protocol);
		}

		static void CloseSocket(Socket Socket)
		{
			PlatformNetwork::Close(Socket);
		}

		static void SetIPv6OnlyEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.IPv6, IPV6_ONLY_OPTION, Value);
		}

		static void SetChecksumEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoChecksum, (!Value ? 0 : 1));
		}

		static void SetBSDUrgentEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BsdUrgent, Value);
		}

		// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
		// will apply a bad effect on the send/receive protocol under TCP connection
		// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
		// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
		static void SetNagleAlgorithmEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, !Value);
		}

		static bool IsSocketReady(Socket Socket)
		{
			return !(Socket.Poll(10, SelectMode.SelectRead) && Socket.Available == 0);
		}

		static IPAddress ResolveDomain(string Domain)
		{
			try
			{
				IPHostEntry entry = Dns.GetHostEntry(Domain);

				if (entry == null || entry.AddressList == null || entry.AddressList.Length == 0)
					return null;

				return entry.AddressList[0];
			}
			catch
			{
				return IPAddress.Parse(Domain);
			}
		}

		static IPAddress MapIPv4ToIPv6(IPAddress IP)
		{
			if (IP.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("IP must be v4");

			return IPAddress.Parse("::ffff:" + IP.ToString());
		}
	};
}

#endif