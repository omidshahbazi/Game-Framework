// Copyright 2019. All Rights Reserved.
using System;
using System.Net;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	static class SocketUtilities
	{

		//
		// Summary:
		//     Close the socket gracefully without lingering.
		DontLinger = -129,
		//
		// Summary:
		//     Enables a socket to be bound for exclusive access.
		ExclusiveAddressUse = -5,
		//
		// Summary:
		//     Record debugging information.
		Debug = 1,
		//
		// Summary:
		//     Specifies the IP options to be inserted into outgoing datagrams.
		IPOptions = 1,
		//
		// Summary:
		//     Send UDP datagrams with checksum set to zero.
		NoChecksum = 1,
		//
		// Summary:
		//     Disables the Nagle algorithm for send coalescing.
		NoDelay = 1,
		//
		// Summary:
		//     The socket is listening.
		AcceptConnection = 2,
		//
		// Summary:
		//     Use urgent data as defined in RFC-1222. This option can be set only once; after
		//     it is set, it cannot be turned off.
		BsdUrgent = 2,
		//
		// Summary:
		//     Use expedited data as defined in RFC-1222. This option can be set only once;
		//     after it is set, it cannot be turned off.
		Expedited = 2,
		//
		// Summary:
		//     Indicates that the application provides the IP header for outgoing datagrams.
		HeaderIncluded = 2,
		//
		// Summary:
		//     Change the IP header type of the service field.
		TypeOfService = 3,
		//
		// Summary:
		//     Set the IP header Time-to-Live field.
		IpTimeToLive = 4,
		//
		// Summary:
		//     Allows the socket to be bound to an address that is already in use.
		ReuseAddress = 4,
		//
		// Summary:
		//     Use keep-alives.
		KeepAlive = 8,
		//
		// Summary:
		//     Set the interface for outgoing multicast packets.
		MulticastInterface = 9,
		//
		// Summary:
		//     An IP multicast Time to Live.
		MulticastTimeToLive = 10,
		//
		// Summary:
		//     An IP multicast loopback.
		MulticastLoopback = 11,
		//
		// Summary:
		//     Add an IP group membership.
		AddMembership = 12,
		//
		// Summary:
		//     Drop an IP group membership.
		DropMembership = 13,
		//
		// Summary:
		//     Do not fragment IP datagrams.
		DontFragment = 14,
		//
		// Summary:
		//     Join a source group.
		AddSourceMembership = 15,
		//
		// Summary:
		//     Do not route; send the packet directly to the interface addresses.
		DontRoute = 16,
		//
		// Summary:
		//     Drop a source group.
		DropSourceMembership = 16,
		//
		// Summary:
		//     Block data from a source.
		BlockSource = 17,
		//
		// Summary:
		//     Unblock a previously blocked source.
		UnblockSource = 18,
		//
		// Summary:
		//     Return information about received packets.
		PacketInformation = 19,
		//
		// Summary:
		//     Set or get the UDP checksum coverage.
		ChecksumCoverage = 20,
		//
		// Summary:
		//     Specifies the maximum number of router hops for an Internet Protocol version
		//     6 (IPv6) packet. This is similar to Time to Live (TTL) for Internet Protocol
		//     version 4.
		HopLimit = 21,
		//
		// Summary:
		//     Enables restriction of a IPv6 socket to a specified scope, such as addresses
		//     with the same link local or site local prefix.This socket option enables applications
		//     to place access restrictions on IPv6 sockets. Such restrictions enable an application
		//     running on a private LAN to simply and robustly harden itself against external
		//     attacks. This socket option widens or narrows the scope of a listening socket,
		//     enabling unrestricted access from public and private users when appropriate,
		//     or restricting access only to the same site, as required. This socket option
		//     has defined protection levels specified in the System.Net.Sockets.IPProtectionLevel
		//     enumeration.
		IPProtectionLevel = 23,
		//
		// Summary:
		//     Indicates if a socket created for the AF_INET6 address family is restricted to
		//     IPv6 communications only. Sockets created for the AF_INET6 address family may
		//     be used for both IPv6 and IPv4 communications. Some applications may want to
		//     restrict their use of a socket created for the AF_INET6 address family to IPv6
		//     communications only. When this value is non-zero (the default on Windows), a
		//     socket created for the AF_INET6 address family can be used to send and receive
		//     IPv6 packets only. When this value is zero, a socket created for the AF_INET6
		//     address family can be used to send and receive packets to and from an IPv6 address
		//     or an IPv4 address. Note that the ability to interact with an IPv4 address requires
		//     the use of IPv4 mapped addresses. This socket option is supported on Windows
		//     Vista or later.
		IPv6Only = 27,
		//
		// Summary:
		//     Permit sending broadcast messages on the socket.
		Broadcast = 32,
		//
		// Summary:
		//     Bypass hardware when possible.
		UseLoopback = 64,
		//
		// Summary:
		//     Linger on close if unsent data is present.
		Linger = 128,
		//
		// Summary:
		//     Receives out-of-band data in the normal data stream.
		OutOfBandInline = 256,
		//
		// Summary:
		//     Specifies the total per-socket buffer space reserved for sends. This is unrelated
		//     to the maximum message size or the size of a TCP window.
		SendBuffer = 4097,
		//
		// Summary:
		//     Specifies the total per-socket buffer space reserved for receives. This is unrelated
		//     to the maximum message size or the size of a TCP window.
		ReceiveBuffer = 4098,
		//
		// Summary:
		//     Specifies the low water mark for Overload:System.Net.Sockets.Socket.Send operations.
		SendLowWater = 4099,
		//
		// Summary:
		//     Specifies the low water mark for Overload:System.Net.Sockets.Socket.Receive operations.
		ReceiveLowWater = 4100,
		//
		// Summary:
		//     Send a time-out. This option applies only to synchronous methods; it has no effect
		//     on asynchronous methods such as the System.Net.Sockets.Socket.BeginSend(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)
		//     method.
		SendTimeout = 4101,
		//
		// Summary:
		//     Receive a time-out. This option applies only to synchronous methods; it has no
		//     effect on asynchronous methods such as the System.Net.Sockets.Socket.BeginSend(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)
		//     method.
		ReceiveTimeout = 4102,
		//
		// Summary:
		//     Get the error status and clear.
		Error = 4103,
		//
		// Summary:
		//     Get the socket type.
		Type = 4104,
		//
		// Summary:
		//     Indicates that the system should defer ephemeral port allocation for outbound
		//     connections. This is equivalent to using the Winsock2 SO_REUSE_UNICASTPORT socket
		//     option.
		ReuseUnicastPort = 12295,
		//
		// Summary:
		//     Updates an accepted socket&#39;s properties by using those of an existing socket.
		//     This is equivalent to using the Winsock2 SO_UPDATE_ACCEPT_CONTEXT socket option
		//     and is supported only on connection-oriented sockets.
		UpdateAcceptContext = 28683,
		//
		// Summary:
		//     Updates a connected socket&#39;s properties by using those of an existing socket.
		//     This is equivalent to using the Winsock2 SO_UPDATE_CONNECT_CONTEXT socket option
		//     and is supported only on connection-oriented sockets.
		UpdateConnectContext = 28688,
		//
		// Summary:
		//     Not supported; will throw a System.Net.Sockets.SocketException if used.
		MaxConnections = int.MaxValue












		private static readonly SocketOptionName IPV6_ONLY_OPTION = (SocketOptionName)27;

		public static Socket CreateSocket(Protocols Protocol)
		{
			ProtocolType protocol = (Protocol == Protocols.TCP ? ProtocolType.Tcp : ProtocolType.Udp);
			SocketType type = (Protocol == Protocols.TCP ? SocketType.Stream : SocketType.Dgram);

			return new Socket(AddressFamily.InterNetworkV6, type, protocol);
		}

		public static void SetIPv6Only(Socket Socket, bool Value)
		{
			Socket.SetSocketOption(SocketOptionLevel.IPv6, IPV6_ONLY_OPTION, Value);
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