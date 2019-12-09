// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef SOCKET_UTILITIES_H
#define SOCKET_UTILITIES_H

#include "Common.h"
#include "PlatformNetwork.h"
#include <string>

namespace GameFramework::Networking
{
	struct IPAddress
	{
	public:
		static const IPAddress Any;
		static const IPAddress AnyV6;

	public:
		IPAddress(void) :
			m_Family(PlatformNetwork::AddressFamilies::InterNetwork)
		{
		}

		IPAddress(PlatformNetwork::AddressFamilies Family, const std::string& IP) :
			m_Family(Family),
			m_IP(IP)
		{
		}

		PlatformNetwork::AddressFamilies GetAddressFamily(void) const
		{
			return m_Family;
		}

		const std::string& GetIP(void) const
		{
			return m_IP;
		}

		void SetIP(const std::string& IP)
		{
			m_IP = IP;
		}

	private:
		PlatformNetwork::AddressFamilies m_Family;
		std::string m_IP;
	};

	struct IPEndPoint
	{
	public:
		IPEndPoint(void) :
			m_Port(0)
		{
		}

		IPEndPoint(const IPAddress& Address, uint16_t Port) :
			m_Address(Address),
			m_Port(Port)
		{
		}

		const IPAddress& GetAddress(void) const
		{
			return m_Address;
		}

		void SetAddress(const IPAddress& Value)
		{
			m_Address = Value;
		}

		uint16_t GetPort(void) const
		{
			return m_Port;
		}

		void SetPort(uint16_t Value)
		{
			m_Port = Value;
		}

		std::string ToString(void) const
		{
			return "[" + m_Address.GetIP() + "]:" + std::to_string(m_Port);
		}

	private:
		IPAddress m_Address;
		uint16_t m_Port;
	};

	typedef PlatformNetwork::Handle Socket;

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

		// After many researches around NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
		// will apply a bad effect on the send/receive protocol under TCP connection
		// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
		// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
		static void SetNagleAlgorithmEnabled(Socket Socket, bool Value);

		static bool GetIsReady(Socket Socket);

		static void Bind(Socket Socket, const IPEndPoint& EndPoint);

		static void Listen(Socket Socket, uint32_t MaxConnections);

		static bool Accept(Socket ListenerSocket, Socket& AcceptedSocket, IPEndPoint& EndPoint);

		static bool Connect(Socket Socket, const IPEndPoint& EndPoint);

		static uint32_t Send(Socket Socket, const std::byte* Buffer, uint32_t Length);

		static bool Select(Socket Socket, PlatformNetwork::SelectModes Mode, uint32_t Timeout);

		static uint64_t GetAvailableBytes(Socket Socket);

		static bool Receive(Socket Socket, std::byte* Buffer, uint32_t& Length);

		static IPAddress ResolveDomain(const std::string& Domain);

		static IPAddress MapIPv4ToIPv6(IPAddress IP);
	};
}

#endif