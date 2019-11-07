using GameFramework.BinarySerializer;
using GameFramework.NetworkingManaged;
using System;

namespace ServerNetworkingManagedTest
{
	class Program
	{
		private static TCPServerSocket server = null;

		static void Main(string[] args)
		{
			server = new TCPServerSocket(24);
			Console.WriteLine("TCPServerSocket created");

			server.MultithreadedCallbacks = false;
			server.MultithreadedReceive = false;
			server.MultithreadedSend = false;

			//server.MultithreadedCallbacks = true;
			//server.MultithreadedReceive = false;
			//server.MultithreadedSend = false;

			//server.MultithreadedCallbacks = false;
			//server.MultithreadedReceive = true;
			//server.MultithreadedSend = false;

			//server.MultithreadedCallbacks = false;
			//server.MultithreadedReceive = false;
			//server.MultithreadedSend = true;

			//server.MultithreadedCallbacks = true;
			//server.MultithreadedReceive = true;
			//server.MultithreadedSend = false;

			//server.MultithreadedCallbacks = true;
			//server.MultithreadedReceive = false;
			//server.MultithreadedSend = true;

			//server.MultithreadedCallbacks = false;
			//server.MultithreadedReceive = true;
			//server.MultithreadedSend = true;

			//server.MultithreadedCallbacks = true;
			//server.MultithreadedReceive = true;
			//server.MultithreadedSend = true;

			server.OnClientConnected += Server_OnClientConnected;
			server.OnClientDisconnected += Server_OnClientDisconnected;
			server.OnBufferReceived += Server_OnBufferReceived;

			server.Bind("::1", 433);
			server.Listen();

			while (true)
			{
				System.Threading.Thread.Sleep(100);

				server.Service();
			}
		}

		private static void Server_OnClientConnected(Client Client)
		{
			Console.WriteLine("Server_OnClientConnected " + Client.Socket.RemoteEndPoint);
		}

		private static void Server_OnClientDisconnected(Client Client)
		{
			Console.WriteLine("Server_OnClientDisconnected " + Client.Socket.RemoteEndPoint);
		}

		private static void Server_OnBufferReceived(Client Sender, BufferStream Buffer)
		{
			Console.WriteLine("Server_OnBufferReceived " + Sender.Socket.RemoteEndPoint + " " + Buffer.Size);

			server.Send(Sender, Buffer);
		}
	}
}
