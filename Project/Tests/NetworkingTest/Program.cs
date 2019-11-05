using GameFramework.NetworkingManaged;
using System;

namespace NetworkingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			TCPServerSocket server = new TCPServerSocket(24);
			server.MultithreadedCallbacks = false;
			server.OnClientConnected += Server_OnClientConnected;
			server.OnClientDisconnected += Server_OnClientDisconnected;
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
	}
}
