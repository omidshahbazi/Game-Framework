#include <UDPClientSocket.h>
#include <iostream>

using namespace GameFramework::Networking;

UDPClientSocket client;

void Server_OnConnected(void)
{
	std::cout << "Server_OnConnected " << std::endl;

	byte data[6000];
	client.Send(data, 6000);
}

void Server_OnConnectionFailed(void)
{
	std::cout << "Server_OnConnectionFailed " << std::endl;
}

void Client_OntDisconnected(void)
{
	std::cout << "Client_OntDisconnected " << std::endl;
}

void Client_OnBufferReceived(BufferStream Buffer)
{
	std::cout << "Server_OnBufferReceived " << " " << Buffer.GetSize() << std::endl;

	client.Send(Buffer.GetBuffer(), Buffer.GetSize());
}

void main()
{
	std::cout << "Client created" << std::endl;

	//client.OnConnected += Server_OnConnected;
	//client.OnConnectionFailed += Server_OnConnectionFailed;
	//client.OnDisconnected += Client_OntDisconnected;
	//client.OnBufferReceived += Client_OnBufferReceived;

	//client.SetMultithreadedCallbacks(false);
	//client.SetMultithreadedReceive(false);
	//client.SetMultithreadedSend(false);

	std::cout << "Client started connecting" << std::endl;
	client.Connect("::1", 80);
	//socket.Connect("fe80::c011:8430:ece7:975f%15", 80);

	while (true)
	{
		this_thread::sleep_for(chrono::milliseconds(100));

		client.Service();
	}

	client.Disconnect();
}