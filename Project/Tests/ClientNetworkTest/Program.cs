using GameFramework.BinarySerializer;
using GameFramework.NetworkingManaged;
using System;

namespace ClientNetworkTest
{
	class Program
	{
		private static TCPClientSocket client = null;

		static void Main(string[] args)
		{
			client = new TCPClientSocket();

			client.OnConnected += Client_OnConnected;
			client.OnConnectionFailed += Client_OnConnectionFailed;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnBufferReceived += Client_OnBufferReceived;

			client.Connect("::1", 433);

			while (true)
			{
				System.Threading.Thread.Sleep(100);

				client.Service();
			}
		}

		private static void Client_OnConnected()
		{
			Console.WriteLine("Client_OnConnected");

			client.Send();
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
		}
	}
}
