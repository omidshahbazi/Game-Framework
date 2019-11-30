#include <TCPServerSocket.h>
#include <iostream>

using namespace GameFramework::Networking;

TCPServerSocket server(32);

void OnClientConnected(Client* Client)
{
	std::cout << "Client Connected" << std::endl;

	//server.OnClientConnected -= OnClientConnected;
}

void main()
{
	std::cout << "TCPServerSocket created" << std::endl;

	server.OnClientConnected += OnClientConnected;

	server.Bind("::0", 80);
	//socket.Bind("fe80::c011:8430:ece7:975f%15", 80);
	std::cout << "TCPServerSocket bound" << std::endl;

	//server.SetMultithreadedCallbacks(false);
	//server.SetMultithreadedReceive(false);
	//server.SetMultithreadedSend(false);

	server.Listen();
	std::cout << "TCPServerSocket started listening" << std::endl;

	while (true)
	{
		//_sleep(100);

		server.Service();
	}

	PlatformNetwork::Shutdown();
}