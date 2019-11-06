using GameFramework.BinarySerializer;
using GameFramework.NetworkingManaged;
using System;

namespace NetworkingTest
{
	class Program
	{
		private static TCPServerSocket server = null;
		static void Main(string[] args)
		{
			server = new TCPServerSocket(24);
			server.MultithreadedCallbacks = false;
			//server.MultithreadedReceive = false;
			//server.MultithreadedSend = false;
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
