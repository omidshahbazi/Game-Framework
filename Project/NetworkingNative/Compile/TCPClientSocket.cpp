// Copyright 2019. All Rights Reserved.
#include "..\Include\TCPClientSocket.h"

namespace GameFramework::Networking
{
	TCPClientSocket::TCPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::TCP),
		m_IsConnecting(false)
	{
	}

	void TCPClientSocket::Service(void)
	{
		ClientSocket::Service();

		if (m_IsConnecting)
			CheckConnectionStatus();
	}

	void TCPClientSocket::Disconnect(void)
	{
		ClientSocket::Disconnect();

		m_IsConnecting = false;
	}

	void TCPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		m_IsConnecting = true;

		SocketUtilities::Connect(GetSocket(), EndPoint);
	}

	void TCPClientSocket::ProcessReceivedBuffer(const BufferStream& Buffer)
	{
		HandleReceivedBuffer(Buffer);
	}

	void TCPClientSocket::CheckConnectionStatus(void)
	{
		if (!GetIsConnected())
		{
			try
			{
				if (SocketUtilities::Select(GetSocket(), PlatformNetwork::SelectModes::SelectWrite, 1000))
				{
					SetIsConnected(true);
					m_IsConnecting = false;

					RunReceiveThread();
					RunSenndThread();

					RaiseOnConnectedEvent();
				}
			}
			catch (PlatformNetwork::SocketException e)
			{
				m_IsConnecting = false;

				RaiseOnConnectionFailedEvent();
			}
		}
	}
}