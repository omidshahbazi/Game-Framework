// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GameFramework.Networking
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
			try
			{
				Socket.Shutdown(SocketShutdown.Both);
			}
			catch { }

			Socket.Close();
		}

		public static void SetBlocking(Socket Socket, bool Value)
		{
			Socket.Blocking = Value;
		}

		public static void SetReceiveBufferSize(Socket Socket, uint Value)
		{
			Socket.ReceiveBufferSize = (int)Value;
		}

		public static void SetSendBufferSize(Socket Socket, uint Value)
		{
			Socket.SendBufferSize = (int)Value;
		}

		public static void SetReceiveTimeout(Socket Socket, uint Value)
		{
			Socket.ReceiveTimeout = (int)Value;
		}

		public static void SetSendTimeout(Socket Socket, uint Value)
		{
			Socket.SendTimeout = (int)Value;
		}

		public static void SetTimeToLive(Socket Socket, ushort Value)
		{
			Socket.Ttl = (short)Value;
		}

		public static void SetIPv6OnlyEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.IPv6, IPV6_ONLY_OPTION, Value);
		}

		public static void SetChecksumEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoChecksum, (!Value ? 0 : 1));
		}

		// After many researches abound NoDelay and the Nagle algorithm, I found out that using this algorithm on TCP
		// will apply a bad effect on the send/receive protocol under TCP connection
		// Altough, NoDelay and the function doesn't work properly as described in MSDN and I desired
		// https://support.microsoft.com/en-us/help/214397/design-issues-sending-small-data-segments-over-tcp-with-winsock
		public static void SetNagleAlgorithmEnabled(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, !Value);
		}

		public static bool GetIsReady(Socket Socket)
		{
			return true;

			//return !(Socket.Poll(10, SelectMode.SelectRead) && Socket.Available == 0);
		}

		public static IPAddress ResolveDomain(string Domain)
		{
			//IPAddress ip;
			//if (IPAddress.TryParse(Domain, out ip))
			//	return ip;

			try
			{
				IPHostEntry entry = Dns.GetHostEntry(Domain);

				if (entry == null || entry.AddressList == null || entry.AddressList.Length == 0)
					return null;

				for (int i = 0; i < entry.AddressList.Length; ++i)
					if (entry.AddressList[i].AddressFamily == AddressFamily.InterNetworkV6)
						return entry.AddressList[i];

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

		public static uint FindOptimumMTU(IPAddress IP, uint Timeout, uint MaxMTU)
		{
			IP = ResolveDomain(IP.ToString());

			Ping ping = new Ping();

			PingOptions options = new PingOptions();
			options.DontFragment = true;

			List<byte> bytesList = new List<byte>((int)MaxMTU);
			for (uint i = 0; i < bytesList.Capacity; ++i)
				bytesList.Add((byte)(i % 255));

			PingReply reply = null;
			do
			{
				reply = ping.Send(IP, (int)Timeout, bytesList.ToArray(), options);

				if (reply == null)
					throw new PingException("Finding optimum MTU failed");

				if (reply.Status == IPStatus.Success)
					break;

				bytesList.RemoveAt(0);

				if (reply.Status == IPStatus.PacketTooBig)
					continue;

				throw new PingException("Finding optimum MTU failed with error " + reply.Status.ToString());

			} while (bytesList.Count != 0);

			return (uint)bytesList.Count;
		}
	}
}