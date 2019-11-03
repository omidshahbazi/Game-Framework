// Copyright 2019. All Rights Reserved.
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public abstract class BaseSocket
	{
		protected Socket Socket
		{
			get;
			private set;
		}

		public BaseSocket(Protocols Type)
		{
			Socket = SocketUtilities.CreateSocket(Type);

			SocketUtilities.SetIPv6Only(Socket, false);
		}
	}
}