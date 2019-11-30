#include <TCPServerSocket.h>
#include <iostream>

using namespace GameFramework::Networking;

TCPServerSocket server(32);

void OnClientConnected(const Client* Client)
{
	std::cout << "OnClientConnected " << Client->GetEndPoint().GetAddress().GetIP() << ":" << Client->GetEndPoint().GetPort() << std::endl;

	//server.OnClientConnected -= OnClientConnected;
}

void OnClientDisconnected(const Client* Client)
{
	std::cout << "OnClientDisconnected " << Client->GetEndPoint().GetAddress().GetIP() << ":" << Client->GetEndPoint().GetPort() << std::endl;
}

void OnBufferReceived(const Client* Client, BufferStream Buffer)
{
	std::cout << "OnBufferReceived " << Client->GetEndPoint().GetAddress().GetIP() << ":" << Client->GetEndPoint().GetPort() << std::endl;
}

void main()
{
	std::cout << "TCPServerSocket created" << std::endl;

	server.OnClientConnected += OnClientConnected;
	server.OnClientDisconnected += OnClientDisconnected;
	server.OnBufferReceived += OnBufferReceived;

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
		this_thread::sleep_for(chrono::seconds(1));

		server.Service();
	}

	PlatformNetwork::Shutdown();
}