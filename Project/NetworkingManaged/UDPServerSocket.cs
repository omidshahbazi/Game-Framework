// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;

namespace GameFramework.NetworkingManaged
{
	public class UDPServerSocket : ServerSocket
	{
		public UDPServerSocket(uint MaxConnection = 32) : base(Protocols.UDP, MaxConnection)
		{
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			throw new System.NotImplementedException();
		}
	}
}