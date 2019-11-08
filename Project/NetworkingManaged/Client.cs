﻿// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Net.Sockets;

namespace GameFramework.NetworkingManaged
{
	public class Client
	{
		public bool IsConnected
		{
			get { return Socket.Connected; }
		}

		public Socket Socket
		{
			get;
			private set;
		}

		public uint RoundTripTime
		{
			get;
			private set;
		}

		public Client(Socket Socket)
		{
			this.Socket = Socket;
		}
	}

	public class ClientList : List<Client>
	{ }
}