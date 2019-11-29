// Copyright 2019. All Rights Reserved.
#include "..\Include\ServerSocket.h"
#include "..\Include\Constants.h"

namespace GameFramework::Networking
{
	ServerSocket::ServerSocket(PlatformNetwork::IPProtocols Type, uint32_t MaxConnection) :
		BaseSocket(Type),
		m_IsBound(false),
		m_MaxConnection(MaxConnection)
	{
	}

	void ServerSocket::Bind(const std::string& Host, uint16_t Port)
	{
		Bind(SocketUtilities::ResolveDomain(Host), Port);
	}

	void ServerSocket::Bind(const IPAddress& IP, uint16_t Port)
	{
		Bind(IPEndPoint(IP, Port));
	}

	void ServerSocket::Bind(const IPEndPoint& EndPoint)
	{
		IPEndPoint endPoint = EndPoint;

		if (endPoint.GetAddress().GetAddressFamily() == PlatformNetwork::AddressFamilies::InterNetwork)
			endPoint.SetAddress(SocketUtilities::MapIPv4ToIPv6(endPoint.GetAddress()));

		m_IsBound = SocketUtilities::Bind(GetSocket(), endPoint);
	}

	void ServerSocket::ServerSocket::UnBind(void)
	{
		m_Clients.clear();

		Shutdown();

		m_IsBound = false;
	}

	void ServerSocket::DisconnectClient(Client* const Client)
	{
		WAIT_FOR_BOOL(m_ClientsLock);

		m_Clients.remove(Client);

		//HandleClientDisconnection(Client);
	}

	void ServerSocket::Listen()
	{
		SocketUtilities::Listen(GetSocket(), m_MaxConnection);

		RunReceiveThread();
		RunSenndThread();
	}

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Length)
	{
		Send(Target, Buffer, 0, Length);
	}

	void ServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		BufferStream buffer = Constants::Packet::CreateOutgoingBufferStream(Length);

		buffer.WriteBytes(Buffer, Index, Length);

		Send(Target, buffer);
	}

	void ServerSocket::Send(const Client* Target, const BufferStream& Buffer)
	{
		AddSendCommand(new ServerSendCommand(Target->GetSocket(), Buffer, GetTimestamp()));
	}

	void ServerSocket::Receive(void)
	{
		IPEndPoint endPoint;
		SocketUtilities::Accept(GetSocket(), endPoint);

		//try
		//{
		//	Socket clientSocket =  Socket.Accept();

		//	Client client = new Client(clientSocket);

		//	lock(clients)
		//		clients.Add(client);

		//	if (MultithreadedCallbacks)
		//	{
		//		if (OnClientConnected != null)
		//			CallbackUtilities.InvokeCallback(OnClientConnected.Invoke, client);
		//	}
		//	else
		//	{
		//		AddEvent(new ClientConnectedEvent(client));
		//	}
		//}
		//catch (SocketException e)
		//{
		//	if (e.SocketErrorCode != SocketError.WouldBlock)
		//		throw e;
		//}

		//lock(clients)
		//{
		//	ClientList disconnectedClients = new ClientList();

		//	for (int i = 0; i < clients.Count; ++i)
		//	{
		//		Client client = clients[i];

		//		try
		//		{
		//			int size = 0;

		//			lock(Socket)
		//			{
		//				if (client.Socket.Available == 0)
		//				{
		//					if (!client.IsReady)
		//					{
		//						disconnectedClients.Add(client);

		//						HandleClientDisconnection(client);
		//					}

		//					continue;
		//				}

		//				size = client.Socket.Receive(ReceiveBuffer);
		//			}

		//			BandwidthIn += (uint)size;

		//			uint index = 0;
		//			while (index != size)
		//			{
		//				uint packetSize = BitConverter.ToUInt32(ReceiveBuffer, (int)index);

		//				index += Constants.Packet.PACKET_SIZE_SIZE;

		//				HandleIncommingBuffer(client, new BufferStream(ReceiveBuffer, index, packetSize));

		//				index += packetSize;
		//			}
		//		}
		//		catch (SocketException e)
		//		{
		//			if (e.SocketErrorCode == SocketError.WouldBlock)
		//				continue;
		//			else if (e.SocketErrorCode == SocketError.ConnectionReset)
		//			{
		//				disconnectedClients.Add(client);

		//				HandleClientDisconnection(client);

		//				continue;
		//			}

		//			throw e;
		//		}
		//		catch (Exception e)
		//		{
		//			throw e;
		//		}
		//	}

		//	for (int i = 0; i < disconnectedClients.Count; ++i)
		//		clients.Remove(disconnectedClients[i]);
		//}
	}

	bool ServerSocket::HandleSendCommand(SendCommand* Command)
	{
		return false;
	}

	void ServerSocket::ProcessEvent(const EventBase& Event)
	{

	}

	double ServerSocket::GetTimestamp(void)
	{
		return 0;
	}
}