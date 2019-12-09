using GameFramework.BinarySerializer;
using GameFramework.Networking;
using System;

namespace ClientNetworkingManagedTest
{
	class Program
	{
		private static UDPClientSocket client = null;

		static void Main(string[] args)
		{
			client = new UDPClientSocket();
			Console.WriteLine("Client created");

			client.MultithreadedCallbacks = true;
			client.MultithreadedReceive = true;
			client.MultithreadedSend = true;

			client.OnConnected += Client_OnConnected;
			client.OnConnectionFailed += Client_OnConnectionFailed;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnBufferReceived += Client_OnBufferReceived;

			Console.WriteLine("Client started connecting");
			client.Connect("127.0.0.1", 80);

			while (true)
			{
				client.Service();

				System.Threading.Thread.Sleep(100);
			}
		}

		private static void Client_OnConnected()
		{
			Console.WriteLine("Client_OnConnected");

			client.Send(System.Text.Encoding.ASCII.GetBytes("asdadjaijijijanciojh82y3	[ncc9n0009u18u24cu4839cyur98ybuc4yc-1nhc1bc3uc127bcn187cb-81c7nc1u-8n147ncb87c4b431-7c17c3n7c13-987c189cn7c17c-81347nc1432n7"), false);
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

			client.Send(Buffer.Buffer, false);
		}
	}
}