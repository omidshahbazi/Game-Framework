// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPServerSocket.h"
#include "..\Include\Constants.h"
#include <Utilities\CRC32.h>
#include <Timing\Time.h>

using namespace GameFramework::Common::Utilities;
using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	UDPServerSocket::UDPServerSocket(void) :
		ServerSocket(PlatformNetwork::IPProtocols::UDP)
	{
	}

	void UDPServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Length, bool Reliable)
	{
		Send(Target, Buffer, 0, Length, Reliable);
	}

	void UDPServerSocket::Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length, bool Reliable)
	{
		UDPClient* target = reinterpret_cast<UDPClient*>(const_cast<Client*>(Target));

		OutgoingUDPPacketsHolder& outgoingHolder = (Reliable ? target->GetOutgoingReliablePacketHolder() : target->GetOutgoingNonReliablePacketHolder());
		IncomingUDPPacketsHolder& incomingHolder = (Reliable ? target->GetIncomingReliablePacketHolder() : target->GetIncomingNonReliablePacketHolder());

		OutgoingUDPPacket* packet = OutgoingUDPPacket::CreateOutgoingBufferStream(outgoingHolder, incomingHolder, Buffer, Index, Length, target->GetMTU(), Reliable);

		SendPacket(target, packet);
	}

	void UDPServerSocket::SendInternal(Client* Client, BufferStream& Buffer)
	{
		AddSendCommand(new ServerSendCommand(Client, Buffer, GetTimestamp()));
	}

	void UDPServerSocket::AcceptClients(void)
	{
	}

	void UDPServerSocket::ReadFromClients(void)
	{
		ReadFromSocket();

		CheckClientsConnection();
	}

	void UDPServerSocket::HandleIncomingBuffer(Client* Client, BufferStream& Buffer)
	{
		byte control = Buffer.ReadByte();

		double time = Time::GetCurrentEpochTime();

		Client->GetStatistics().SetLastTouchTime(time);

		UDPClient* client = reinterpret_cast<UDPClient*>(Client);

		if (control == Constants::Control::BUFFER)
		{
			if (!client->GetIsConnected())
				return;

			BufferStream buffer = Packet::CreateIncomingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(Client, buffer);
		}
		else if (control == Constants::Control::HANDSHAKE)
		{
			client->SetIsConnected(true);

			uint32_t mtu = Buffer.ReadUInt32();
			client->SetMTU(mtu);

			m_Clients.push_back(client);

			BufferStream buffer = Packet::CreateHandshakeBackBufferStream(client->GetStatistics().GetPacketCountRate());

			SendInternal(Client, buffer);
		}
		else if (control == Constants::Control::PING)
		{
			if (!client->GetIsConnected())
				return;

			double sendTime = Buffer.ReadFloat64();

			client->GetStatistics().SetLatency((time - sendTime) * 1000);

			BufferStream pingBuffer = OutgoingUDPPacket::CreatePingBufferStream(client->GetOutgoingReliablePacketHolder(), client->GetIncomingReliablePacketHolder(), client->GetOutgoingNonReliablePacketHolder(), client->GetIncomingNonReliablePacketHolder());

			SendInternal(Client, pingBuffer);

			uint64_t lastAckID = Buffer.ReadUInt64();
			uint32_t ackMask = Buffer.ReadUInt32();
			client->GetOutgoingReliablePacketHolder().SetLastAckID(lastAckID);
			client->GetOutgoingReliablePacketHolder().SetAckMask(ackMask);

			lastAckID = Buffer.ReadUInt64();
			ackMask = Buffer.ReadUInt32();
			client->GetOutgoingNonReliablePacketHolder().SetLastAckID(lastAckID);
			client->GetOutgoingNonReliablePacketHolder().SetAckMask(ackMask);
		}
	}

	bool UDPServerSocket::HandleSendCommand(SendCommand* Command)
	{
		ServerSendCommand* sendCommand = reinterpret_cast<ServerSendCommand*>(Command);
		UDPClient* client = reinterpret_cast<UDPClient*>(sendCommand->GetClient());

		if (!client->GetIsReady())
			return false;

		client->GetStatistics().AddBandwidthOut(Command->GetBuffer().GetSize());

		SendOverSocket(client->GetEndPoint(), Command->GetBuffer());

		return true;
	}

	void UDPServerSocket::ProcessReceivedBuffer(Client* Sender, BufferStream& Buffer)
	{
		uint64_t lastAckID = Buffer.ReadUInt64();
		uint32_t ackMask = Buffer.ReadUInt32();
		bool isReliable = Buffer.ReadBool();
		uint64_t packetID = Buffer.ReadUInt64();
		uint16_t sliceCount = Buffer.ReadUInt16();
		uint16_t sliceIndex = Buffer.ReadUInt16();

		BufferStream buffer(Buffer.GetBuffer(), Constants::UDP::PACKET_HEADER_SIZE, Buffer.GetSize() - Constants::UDP::PACKET_HEADER_SIZE);

		UDPClient* client = reinterpret_cast<UDPClient*>(Sender);

		IncomingUDPPacketsHolder& incomingHolder = (isReliable ? client->GetIncomingReliablePacketHolder() : client->GetIncomingNonReliablePacketHolder());

		IncomingUDPPacket* packet = incomingHolder.GetPacket(packetID);
		if (packet == nullptr)
		{
			packet = new IncomingUDPPacket(packetID, sliceCount);
			incomingHolder.AddPacket(packet);
		}

		packet->SetSliceBuffer(sliceIndex, buffer);

		if (packet->GetIsCompleted())
		{
			if (incomingHolder.GetLastID() < packet->GetID())
				incomingHolder.SetLastID(packet->GetID());

			if (isReliable)
				ProcessIncomingReliablePackets(client);
			else
				ProcessIncomingNonReliablePacket(client, *packet);
		}

		OutgoingUDPPacketsHolder& outgoingHolder = (isReliable ? client->GetOutgoingReliablePacketHolder() : client->GetOutgoingNonReliablePacketHolder());
		outgoingHolder.SetLastAckID(lastAckID);
		outgoingHolder.SetAckMask(ackMask);

		if (isReliable)
			ProcessOutgoingReliablePackets(client);
		else
			ProcessOutgoingNonReliablePackets(client);
	}

	void UDPServerSocket::ReadFromSocket(void)
	{
		IPEndPoint ipEndPoint(IPAddress::AnyV6, 0);

		try
		{
			uint32_t size = Constants::RECEIVE_BUFFER_SIZE;

			//lock(Socket)
			{
				if (SocketUtilities::GetAvailableBytes(GetSocket()) == 0)
					return;

				if (!SocketUtilities::ReceiveFrom(GetSocket(), GetReceiveBuffer(), size, ipEndPoint))
					return;
			}

			UDPClient* client = GetOrAddClient(ipEndPoint);

			client->GetStatistics().AddReceivedPacketFromLastSecond();

			ServerSocket::ProcessReceivedBuffer(client, size);
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.GetError() == PlatformNetwork::Errors::WouldBlock)
				return;
			else if (e.GetError() == PlatformNetwork::Errors::ConnectionReset)
			{
				if (ipEndPoint.GetAddress() == IPAddress::AnyV6)
					return;

				int hash = GetIPEndPointHash(ipEndPoint);

				//lock(clients)
				{
					UDPClient* client = GetOrAddClient(ipEndPoint);

					m_Clients.remove(client);
					m_ClientsMap.erase(hash);

					HandleClientDisconnection(client);
				}

				return;
			}

			throw e;
		}
		catch (std::exception e)
		{
			throw e;
		}
	}

	void UDPServerSocket::CheckClientsConnection(void)
	{
		for (int i = 0; i < m_Clients.size(); ++i)
		{
			UDPClient* client = m_Clients[i];

			if (client->GetIsReady())
				continue;

			int hash = GetIPEndPointHash(client->GetEndPoint());

			m_Clients.remove(client);
			m_ClientsMap.erase(hash);

			HandleClientDisconnection(client);
		}
	}

	void UDPServerSocket::SendPacket(Client* Client, OutgoingUDPPacket* Packet)
	{
		for (uint16_t i = 0; i < Packet->GetSliceCount(); ++i)
			SendInternal(Client, Packet->GetSliceBuffer(i));
	}

	void UDPServerSocket::ProcessIncomingReliablePackets(UDPClient* Sender)
	{
		//IncomingUDPPacketsHolder::ProcessReliablePackets(Sender->GetIncomingReliablePacketHolder(), [&](BufferStream& Buffer)
		//	{
		//		HandleReceivedBuffer(Sender, Buffer);
		//	});
	}

	void UDPServerSocket::ProcessIncomingNonReliablePacket(UDPClient* Sender, IncomingUDPPacket& Packet)
	{
		auto callback = [this, Sender](BufferStream& Buffer)
		{
			HandleReceivedBuffer(Sender, Buffer);
		};

		//IncomingUDPPacketsHolder::ProcessNonReliablePacket(Sender->GetIncomingNonReliablePacketHolder(), Packet, std::bind(&callback, this, std::placeholders::_1));
	}

	void UDPServerSocket::ProcessOutgoingReliablePackets(UDPClient* Target)
	{
		//OutgoingUDPPacketsHolder::ProcessReliablePackets(Target->GetOutgoingReliablePacketHolder(), std::bind(&UDPServerSocket::SendPacket, this, std::placeholders::_1));
	}

	void UDPServerSocket::ProcessOutgoingNonReliablePackets(UDPClient* Target)
	{
		//OutgoingUDPPacketsHolder::ProcessNonReliablePackets(Target->GetOutgoingNonReliablePacketHolder(), std::bind(&UDPServerSocket::SendPacket, this, std::placeholders::_1));
	}

	UDPServerSocket::UDPClient* UDPServerSocket::GetOrAddClient(const IPEndPoint& EndPoint)
	{
		int hash = GetIPEndPointHash(EndPoint);

		if (m_ClientsMap.find(hash) != m_ClientsMap.end())
			return m_ClientsMap[hash];

		UDPClient* client = new UDPClient(EndPoint);
		client->GetStatistics().SetPacketCountRate(GetPacketCountRate());

		m_ClientsMap[hash] = client;

		return client;
	}

	uint32_t GameFramework::Networking::UDPServerSocket::GetIPEndPointHash(const IPEndPoint& EndPoint)
	{
		return CRC32::CalculateHash(reinterpret_cast<const byte*>(&EndPoint), sizeof(IPEndPoint));
	}
}