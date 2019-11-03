// Copyright 2019. All Rights Reserved.

namespace GameFramework.NetworkingManaged
{
	public class TCPClientSocket : ClientSocket
	{
		public TCPClientSocket() : base(Protocols.TCP)
		{
		}
	}
}