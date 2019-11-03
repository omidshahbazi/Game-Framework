using GameFramework.NetworkingManaged;
using System;

namespace NetworkingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			TCPServerSocket server = new TCPServerSocket(24);
			server.Bind("::1", 433);


			TCPClientSocket client = new TCPClientSocket();
			client.Connect("::1", 433);
		}
	}
}
