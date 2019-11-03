// Copyright 2019. All Rights Reserved.

namespace GameFramework.NetworkingManaged
{
	public class UDPServerSocket : ServerSocket
	{
		public UDPServerSocket(uint MaxConnection = 32) : base(Protocols.UDP, MaxConnection)
		{
		}
	}
}