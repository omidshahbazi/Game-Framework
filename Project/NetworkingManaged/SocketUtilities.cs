// Copyright 2019. All Rights Reserved.
using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	static class SocketUtilities
	{
		private static readonly SocketOptionName IPV6_ONLY_OPTION = (SocketOptionName)27;

		public static Socket CreateSocket(Protocols Protocol)
		{
			ProtocolType protocol = (Protocol == Protocols.TCP ? ProtocolType.Tcp : ProtocolType.Udp);
			SocketType type = (Protocol == Protocols.TCP ? SocketType.Stream : SocketType.Dgram);

			return new Socket(AddressFamily.InterNetworkV6, type, protocol);
		}

		public static void CloseSocket(Socket Socket)
		{
			Socket.Shutdown(SocketShutdown.Both);
			Socket.Close();
		}

		public static void SetIPv6OnlyEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.IPv6, IPV6_ONLY_OPTION, Value);
		}

		public static void SetChecksumEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoChecksum, (!Value ? 0 : 1));
		}

		public static void SetDelayEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, !Value);
		}

		public static IPAddress ResolveDomain(string Domain)
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

		public static IPAddress MapIPv4ToIPv6(IPAddress IP)
		{
			if (IP.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("IP must be v4");

			return IPAddress.Parse("::ffff:" + IP.ToString());
		}
	}
}