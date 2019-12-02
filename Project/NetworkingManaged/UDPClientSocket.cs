// Copyright 2019. All Rights Reserved.
using System.Net;
using GameFramework.BinarySerializer;

namespace GameFramework.Networking
{
	public class UDPClientSocket : ClientSocket
	{
		public UDPClientSocket() : base(Protocols.UDP)
		{
		}

		protected override void ConnectInternal(IPEndPoint EndPoint)
		{
			throw new System.NotImplementedException();
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			throw new System.NotImplementedException();
		}
	}
}