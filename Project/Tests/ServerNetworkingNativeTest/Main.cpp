#include <TCPServerSocket.h>
#include <iostream>

using namespace GameFramework::Networking;

TCPServerSocket server;

void Server_OnClientConnected(const Client* Client)
{
	std::cout << "Server_OnClientConnected " << Client->GetEndPoint().ToString() << std::endl;

	//server.OnClientConnected -= Server_OnClientConnected;
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
	std::cout << "Server created" << std::endl;

	server.OnClientConnected += Server_OnClientConnected;
	server.OnClientDisconnected += Server_OnClientDisconnected;
	server.OnBufferReceived += Server_OnBufferReceived;

	//server.SetMultithreadedCallbacks(false);
	//server.SetMultithreadedReceive(false);
	//server.SetMultithreadedSend(false);

	server.Bind("::0", 80);
	//socket.Bind("fe80::c011:8430:ece7:975f%15", 80);
	std::cout << "Server bound" << std::endl;

	server.Listen();
	std::cout << "Server started listening" << std::endl;

	while (true)
	{
		this_thread::sleep_for(chrono::milliseconds(100));

		server.Service();
	}

	server.UnBind();
}