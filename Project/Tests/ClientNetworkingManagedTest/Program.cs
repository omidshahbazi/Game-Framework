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

			client.MultithreadedCallbacks = false;
			client.MultithreadedReceive = false;
			client.MultithreadedSend = false;

			//client.MultithreadedCallbacks = true;
			//client.MultithreadedReceive = false;
			//client.MultithreadedSend = false;

			//client.MultithreadedCallbacks = false;
			//client.MultithreadedReceive = true;
			//client.MultithreadedSend = false;

			//client.MultithreadedCallbacks = false;
			//client.MultithreadedReceive = false;
			//client.MultithreadedSend = true;

			//client.MultithreadedCallbacks = true;
			//client.MultithreadedReceive = true;
			//client.MultithreadedSend = false;

			//client.MultithreadedCallbacks = true;
			//client.MultithreadedReceive = false;
			//client.MultithreadedSend = true;

			//client.MultithreadedCallbacks = false;
			//client.MultithreadedReceive = true;
			//client.MultithreadedSend = true;

			client.MultithreadedCallbacks = true;
			client.MultithreadedReceive = true;
			client.MultithreadedSend = true;

			client.OnConnected += Client_OnConnected;
			client.OnConnectionFailed += Client_OnConnectionFailed;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnBufferReceived += Client_OnBufferReceived;

			client.Connect("::1", 80);
			Console.WriteLine("Client started connecting");

			while (true)
			{
				client.Service();

				System.Threading.Thread.Sleep(100);
			}
		}

		private static void Client_OnConnected()
		{
			Console.WriteLine("Client_OnConnected");

			client.Send(new byte[] { 10, 22, 16 });
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

			client.Send(Buffer.Buffer);
		}
	}
}
