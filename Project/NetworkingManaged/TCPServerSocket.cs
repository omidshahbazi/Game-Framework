// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;

namespace GameFramework.NetworkingManaged
{
	public class TCPServerSocket : ServerSocket
	{
		public TCPServerSocket(uint MaxConnection = 32) : base(Protocols.TCP, MaxConnection)
		{
		}

		protected override void ProcessReceivedBuffer(Client Sender, BufferStream Buffer)
		{
			HandleReceivedBuffer(Sender, Buffer);
		}
	}
}