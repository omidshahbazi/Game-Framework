// Copyright 2019. All Rights Reserved.

namespace GameFramework.NetworkingManaged
{
	public class UDPClientSocket : ClientSocket
	{
		public UDPClientSocket() : base(Protocols.UDP)
		{
		}
	}
}