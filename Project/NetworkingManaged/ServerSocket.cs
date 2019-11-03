// Copyright 2019. All Rights Reserved.
using System.Net;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public class ServerSocket
	{
		private Socket socket = null;

		public ServerSocket(Protocols Type)
		{
			socket = SocketUtilities.CreateSocket(Type);

			SocketUtilities.SetIPv6Only(socket, false);
		}

		public void Bind(string Host, ushort Port)
		{
			IPAddress ip = SocketUtilities.ResolveDomain(Host);

			if (ip.AddressFamily == AddressFamily.InterNetwork)
				ip = SocketUtilities.MapIPv4ToIPv6(ip);


			socket.Bind(new IPEndPoint(ip, Port));
		}
	}
}