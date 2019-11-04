using GameFramework.NetworkingManaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
