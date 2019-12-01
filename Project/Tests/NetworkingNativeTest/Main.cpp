#include <TCPServerSocket.h>
#include <iostream>

using namespace GameFramework::Networking;

TCPServerSocket server(32);

void Server_OnClientConnected(const Client* Client)
{
	std::cout << "Server_OnClientConnected " << Client->GetEndPoint().ToString() << std::endl;

	//server.OnClientConnected -= OnClientConnected;
}

void Server_OnClientDisconnected(const Client* Client)
{
	std::cout << "Server_OnClientDisconnected " << Client->GetEndPoint().ToString() << std::endl;
}

void Server_OnBufferReceived(const Client* Client, BufferStream Buffer)
{
	std::cout << "Server_OnBufferReceived " << Client->GetEndPoint().ToString() << " " << Buffer.GetSize() << std::endl;

	server.Send(Client, Buffer.GetBuffer(), Buffer.GetSize());
}

void main()
{
	std::cout << "TCPServerSocket created" << std::endl;

	server.OnClientConnected += Server_OnClientConnected;
	server.OnClientDisconnected += Server_OnClientDisconnected;
	server.OnBufferReceived += Server_OnBufferReceived;

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
		this_thread::sleep_for(chrono::milliseconds(1));

		server.Service();
	}

	PlatformNetwork::Shutdown();
}