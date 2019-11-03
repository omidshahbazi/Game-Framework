// Copyright 2019. All Rights Reserved.

namespace GameFramework.NetworkingManaged
{
	public class TCPServerSocket : ServerSocket
	{
		public TCPServerSocket(uint MaxConnection = 32) : base(Protocols.TCP, MaxConnection)
		{
		}
	}
}