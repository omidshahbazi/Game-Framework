
#include <ServerSocket.h>

using namespace GameFramework::Networking;



void main()
{
	ServerSocket socket(PlatformNetwork::IPProtocols::TCP, 32);
	//socket.Bind("193.176.243.149", 80);
	socket.Bind("google.com", 80);

	PlatformNetwork::Shutdown();
}