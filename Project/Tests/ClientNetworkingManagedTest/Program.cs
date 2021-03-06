﻿using GameFramework.BinarySerializer;
using GameFramework.Networking;
using System;

namespace ClientNetworkingManagedTest
{
	class Program
	{
		private static TCPClientSocket client = null;

		static void Main(string[] args)
		{
			client = new TCPClientSocket();
			Console.WriteLine("Client created");

			//client.PacketLossSimulation = 0.5F;

			client.MultithreadedCallbacks = true;
			client.MultithreadedReceive = true;
			client.MultithreadedSend = true;

			client.OnConnected += Client_OnConnected;
			client.OnConnectionFailed += Client_OnConnectionFailed;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnBufferReceived += Client_OnBufferReceived;

			Console.WriteLine("Client started connecting");
			client.Connect("::1", 88);

			while (true)
			{
				client.Service();

				System.Threading.Thread.Sleep(100);
			}
		}

		private static void Client_OnConnected()
		{
			Console.WriteLine("Client_OnConnected");

			client.Send(System.Text.Encoding.ASCII.GetBytes("asdadjaijijijanciojh82y3	[ncc9n0009u18u24cu4839cyur98ybuc4yc-1nhc1bc3uc127bcn187cb-81c7nc1u-8n147ncb87c4b431-7c17c3n7c13-987c189cn7c17c-81347nc1432n7"));
			//client.Send(new byte[] { 1, 2, 3 }, true);
		}

		private static void Client_OnConnectionFailed()
		{
			Console.WriteLine("Client_OnConnectionFailed");
		}

		private static void Client_OnDisconnected()
		{
			Console.WriteLine("Client_OnDisconnected");
		}

		private static void Client_OnBufferReceived(BufferStream Buffer)
		{
			Console.WriteLine("Client_OnBufferReceived " + Buffer.Size);

			//client.PacketLossSimulation = 0.5F;
			client.Send(Buffer.Buffer);

			//client.PacketLossSimulation = 0;
			//client.Send(Buffer.Buffer);
		}
	}
}