using GameFramework.NetworkingManaged;

namespace NetworkingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			TCPServerSocket server = new TCPServerSocket(24);
			server.Bind("::1", 433);
			server.Listen();

			while (true)
			{
				System.Threading.Thread.Sleep(100);

				server.Service();
			}
		}
	}
}
