﻿using GameFramework.BinarySerializer;
using GameFramework.Networking;
using System;

namespace ServerNetworkingManagedTest
{
	class Program
	{
		private static UDPServerSocket server = null;

		static void Main(string[] args)
		{
			server = new UDPServerSocket();
			Console.WriteLine("Server created");

			server.ReceiveBufferSize = server.SendBufferSize = 10485760;

			server.MultithreadedCallbacks = true;
			server.MultithreadedReceive = true;
			server.MultithreadedSend = true;

			server.OnClientConnected += Server_OnClientConnected;
			server.OnClientDisconnected += Server_OnClientDisconnected;
			server.OnBufferReceived += Server_OnBufferReceived;

			server.Bind("::0", 5000);
			Console.WriteLine("Server bound");

			server.Listen();
			Console.WriteLine("Server started listening");

			while (true)
			{
				System.Threading.Thread.Sleep(100);

				server.Service();
			}
		}

		private static void Server_OnClientConnected(Client Client)
		{
			Console.WriteLine("Server_OnClientConnected " + Client.EndPoint);
		}

		private static void Server_OnClientDisconnected(Client Client)
		{
			Console.WriteLine("Server_OnClientDisconnected " + Client.EndPoint);
		}

		private static void Server_OnBufferReceived(Client Sender, BufferStream Buffer)
		{
			Console.WriteLine("Server_OnBufferReceived " + Sender.EndPoint + " " + Buffer.Size);

			server.Send(Sender, Buffer.Buffer);
		}
	}
}
