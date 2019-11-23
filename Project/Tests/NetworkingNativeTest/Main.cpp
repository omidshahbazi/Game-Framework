
#include <ServerSocket.h>

using namespace GameFramework::Networking;



void main()
{
	ServerSocket socket(PlatformNetwork::IPProtocols::TCP, 32);
	socket.Bind("127.0.0.1", 80);
}