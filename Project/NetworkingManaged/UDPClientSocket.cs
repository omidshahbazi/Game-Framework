// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;

namespace GameFramework.NetworkingManaged
{
	public class UDPClientSocket : ClientSocket
	{
		public UDPClientSocket() : base(Protocols.UDP)
		{
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			throw new System.NotImplementedException();
		}
	}
}