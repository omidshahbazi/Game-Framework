// Copyright 2019. All Rights Reserved.

using GameFramework.BinarySerializer;

namespace GameFramework.Networking
{
	public class TCPClientSocket : ClientSocket
	{
		public TCPClientSocket() : base(Protocols.TCP)
		{
		}

		protected override void ProcessReceivedBuffer(BufferStream Buffer)
		{
			HandleReceivedBuffer(Buffer);
		}
	}
}