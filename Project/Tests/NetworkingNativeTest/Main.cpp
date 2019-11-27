
#include <ServerSocket.h>

using namespace GameFramework::Networking;



void main()
{
	ServerSocket socket(PlatformNetwork::IPProtocols::TCP, 32);
	socket.Bind("193.176.243.149", 80);
	//socket.Bind("fe80::c011:8430:ece7:975f%15", 80);

	PlatformNetwork::Shutdown();
}