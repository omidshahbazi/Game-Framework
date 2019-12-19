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
			if (!client->GetIsConnected)
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
		ServerSendCommand sendCommand = (ServerSendCommand)Command;
		UDPClient client = (UDPClient)sendCommand.Client;

		if (!client.IsReady)
			return false;

		client.Statistics.AddBandwidthOut(Command.Buffer.Size);

		SendOverSocket(client.EndPoint, Command.Buffer);

		return true;
	}

	void UDPServerSocket::ProcessReceivedBuffer(Client* Sender, const BufferStream& Buffer)
	{
		ulong lastAckID = Buffer.ReadUInt64();
		uint ackMask = Buffer.ReadUInt32();
		bool isReliable = Buffer.ReadBool();
		ulong packetID = Buffer.ReadUInt64();
		ushort sliceCount = Buffer.ReadUInt16();
		ushort sliceIndex = Buffer.ReadUInt16();

		BufferStream buffer = new BufferStream(Buffer.Buffer, Constants.UDP.PACKET_HEADER_SIZE, Buffer.Size - Constants.UDP.PACKET_HEADER_SIZE);

		UDPClient client = (UDPClient)Sender;

		IncomingUDPPacketsHolder incomingHolder = (isReliable ? client.IncomingReliablePacketHolder : client.IncomingNonReliablePacketHolder);

		IncomingUDPPacket packet = incomingHolder.GetPacket(packetID);
		if (packet == null)
		{
			packet = new IncomingUDPPacket(packetID, sliceCount);
			incomingHolder.AddPacket(packet);
		}

		packet.SetSliceBuffer(sliceIndex, buffer);

		if (packet.IsCompleted)
		{
			if (incomingHolder.LastID < packet.ID)
				incomingHolder.SetLastID(packet.ID);

			if (isReliable)
				ProcessIncomingReliablePackets(client);
			else
				ProcessIncomingNonReliablePacket(client, packet);
		}

		OutgoingUDPPacketsHolder outgoingHolder = (isReliable ? client.OutgoingReliablePacketHolder : client.OutgoingNonReliablePacketHolder);
		outgoingHolder.SetLastAckID(lastAckID);
		outgoingHolder.SetAckMask(ackMask);

		if (isReliable)
			ProcessOutgoingReliablePackets(client);
		else
			ProcessOutgoingNonReliablePackets(client);
	}

	void UDPServerSocket::ReadFromSocket(void)
	{
		IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

		try
		{
			int size = 0;
			EndPoint endPoint = ipEndPoint;

			lock(Socket)
			{
				if (Socket.Available == 0)
					return;

				size = Socket.ReceiveFrom(ReceiveBuffer, ref endPoint);

				ipEndPoint = (IPEndPoint)endPoint;
			}

			UDPClient client = GetOrAddClient(ipEndPoint);

			client.Statistics.AddReceivedPacketFromLastSecond();

			ProcessReceivedBuffer(client, (uint)size);
		}
		catch (PlatformNetwork::SocketException e)
		{
			if (e.SocketErrorCode == SocketError.WouldBlock)
				return;
			else if (e.SocketErrorCode == SocketError.ConnectionReset)
			{
				if (ipEndPoint.Address == IPAddress.IPv6Any)
					return;

				int hash = GetIPEndPointHash(ipEndPoint);

				lock(clients)
				{
					UDPClient client = GetOrAddClient(ipEndPoint);

					clients.Remove(client);
					clientsMap.Remove(hash);

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
		for (ushort i = 0; i < Packet.SliceBuffers.Length; ++i)
			SendInternal(Target, Packet.SliceBuffers[i]);
	}

	void UDPServerSocket::ProcessIncomingReliablePackets(UDPClient* Sender)
	{
		IncomingUDPPacketsHolder::ProcessReliablePackets(Sender->GetIncomingReliablePacketHolder(), [&](BufferStream& Buffer)
			{
				HandleReceivedBuffer(Sender, Buffer);
			});
	}

	void UDPServerSocket::ProcessIncomingNonReliablePacket(UDPClient* Sender, IncomingUDPPacket Packet)
	{
		IncomingUDPPacketsHolder::ProcessNonReliablePacket(Sender->GetIncomingNonReliablePacketHolder(), Packet, std::bind(&UDPServerSocket::HandleReceivedBuffer, this, std::placeholders::_1));
	}

	void UDPServerSocket::ProcessOutgoingReliablePackets(UDPClient* Target)
	{
		OutgoingUDPPacketsHolder::ProcessReliablePackets(Target->GetOutgoingReliablePacketHolder(), std::bind(&UDPServerSocket::SendPacket, this, std::placeholders::_1));
	}

	void UDPServerSocket::ProcessOutgoingNonReliablePackets(UDPClient* Target)
	{
		OutgoingUDPPacketsHolder::ProcessNonReliablePackets(Target->GetOutgoingNonReliablePacketHolder(), std::bind(&UDPServerSocket::SendPacket, this, std::placeholders::_1));
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