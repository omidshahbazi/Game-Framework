
#include <ServerSocket.h>

using namespace GameFramework::Networking;



void main()
{
	ServerSocket socket(PlatformNetwork::IPProtocols::TCP, 32);
	socket.Bind("::1", 80);

	PlatformNetwork::Shutdown();
}