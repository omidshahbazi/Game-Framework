using GameFramework.BinarySerializer;
using GameFramework.NetworkingManaged;
using System;

namespace ClientNetworkingManagedTest
{
	class Program
	{
		private static TCPClientSocket client = null;

		static void Main(string[] args)
		{
			client = new TCPClientSocket();
			Console.WriteLine("TCPClientSocket created");

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

			//client.MultithreadedCallbacks = true;
			//client.MultithreadedReceive = true;
			//client.MultithreadedSend = true;

			client.OnConnected += Client_OnConnected;
			client.OnConnectionFailed += Client_OnConnectionFailed;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnBufferReceived += Client_OnBufferReceived;

			client.Connect("::1", 433);
			Console.WriteLine("TCPServerSocket started connecting");

			while (true)
			{
				System.Threading.Thread.Sleep(100);

				client.Service();
			}
		}

		private static void Client_OnConnected()
		{
			Console.WriteLine("Client_OnConnected");

			client.Send(new byte[] { 10, 22 });
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
