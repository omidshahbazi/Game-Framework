using GameFramework.NetworkingManaged;
using System;

namespace NetworkingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			ServerSocket server = new ServerSocket(Protocols.TCP);
			server.Bind("127.0.0.1", 433);


			ClientSocket client = new ClientSocket(Protocols.TCP);

			Console.WriteLine("Hello World!");
		}
	}
}
