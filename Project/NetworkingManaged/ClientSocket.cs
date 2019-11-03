// Copyright 2019. All Rights Reserved.
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public class ClientSocket
	{
		private Socket socket = null;

		public ClientSocket(Protocols Type)
		{
			socket = SocketUtilities.CreateSocket(Type);
		}
	}
}