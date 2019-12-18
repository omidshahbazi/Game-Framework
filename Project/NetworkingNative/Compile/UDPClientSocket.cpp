// Copyright 2019. All Rights Reserved.
#include "..\Include\UDPClientSocket.h"
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;

namespace GameFramework::Networking
{
	UDPClientSocket::UDPClientSocket(void) :
		ClientSocket(PlatformNetwork::IPProtocols::UDP),
		m_MTU(0)
	{
	}

	void UDPClientSocket::Send(byte* const Buffer, uint32_t Length, bool Reliable)
	{
		Send(Buffer, 0, Length, Reliable);
	}

	void UDPClientSocket::Send(byte* const Buffer, uint32_t Index, uint32_t Length, bool Reliable)
	{
		OutgoingUDPPacketsHolder& outgoingHolder = (Reliable ? m_OutgoingReliablePacketHolder : m_OutgoingNonReliablePacketHolder);
		IncomingUDPPacketsHolder& incomingHolder = (Reliable ? m_IncomingReliablePacketHolder : m_IncomingNonReliablePacketHolder);

		OutgoingUDPPacket* packet = OutgoingUDPPacket::CreateOutgoingBufferStream(outgoingHolder, incomingHolder, Buffer, Index, Length, m_MTU, Reliable);

		SendPacket(packet);
	}

	void UDPClientSocket::SendInternal(BufferStream& Buffer)
	{
		AddSendCommand(new SendCommand(Buffer, GetTimestamp()));
	}

	void UDPClientSocket::ConnectInternal(const IPEndPoint& EndPoint)
	{
		SocketUtilities::Connect(GetSocket(), EndPoint);

		m_MTU = SocketUtilities::FindOptimumMTU(EndPoint.GetAddress(), 1000, Constants::UDP::MAX_MTU);

		BufferStream buffer = Packet::CreateHandshakeBufferStream(m_MTU);
		SendInternal(buffer);

		RunReceiveThread();
		RunSenndThread();
	}

	void UDPClientSocket::HandleIncomingBuffer(BufferStream& Buffer)
	{
		GetStatistics().SetLastTouchTime(Time::GetCurrentEpochTime());

		byte control = Buffer.ReadByte();

		if (control == Constants::Control::BUFFER)
		{
			BufferStream buffer = Packet::CreateIncomingBufferStream(Buffer.GetBuffer(), Buffer.GetSize());

			ProcessReceivedBuffer(buffer);
		}
		else if (control == Constants::Control::HANDSHAKE_BACK)
		{
			GetStatistics().SetPacketCountRate(Buffer.ReadUInt32());

			SetIsConnected(true);

			RaiseOnConnectedEvent();
		}
		else if (control == Constants::Control::PING)
		{
			HandlePingPacket(Buffer);
		}
	}

	void UDPClientSocket::ProcessReceivedBuffer(BufferStream& Buffer)
	{
		uint64_t lastAckID = Buffer.ReadUInt64();
		uint32_t ackMask = Buffer.ReadUInt32();
		bool isReliable = Buffer.ReadBool();
		uint64_t packetID = Buffer.ReadUInt64();
		uint16_t sliceCount = Buffer.ReadUInt16();
		uint16_t sliceIndex = Buffer.ReadUInt16();

		BufferStream buffer(Buffer.GetBuffer(), Constants::UDP::PACKET_HEADER_SIZE, Buffer.GetSize() - Constants::UDP::PACKET_HEADER_SIZE);

		IncomingUDPPacketsHolder& incomingHolder = (isReliable ? m_IncomingReliablePacketHolder : m_IncomingNonReliablePacketHolder);

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
				ProcessIncomingReliablePackets();
			else
				ProcessIncomingNonReliablePacket(*packet);
		}

		OutgoingUDPPacketsHolder& outgoingHolder = (isReliable ? m_OutgoingReliablePacketHolder : m_OutgoingNonReliablePacketHolder);
		outgoingHolder.SetLastAckID(lastAckID);
		outgoingHolder.SetAckMask(ackMask);

		if (isReliable)
			ProcessOutgoingReliablePackets();
		else
			ProcessOutgoingNonReliablePackets();
	}

	void UDPClientSocket::HandlePingPacketPayload(BufferStream& Buffer)
	{
		ClientSocket::HandlePingPacketPayload(Buffer);

		uint64_t lastAckID = Buffer.ReadUInt64();
		uint32_t ackMask = Buffer.ReadUInt32();
		m_OutgoingReliablePacketHolder.SetLastAckID(lastAckID);
		m_OutgoingReliablePacketHolder.SetAckMask(ackMask);

		lastAckID = Buffer.ReadUInt64();
		ackMask = Buffer.ReadUInt32();
		m_OutgoingNonReliablePacketHolder.SetLastAckID(lastAckID);
		m_OutgoingNonReliablePacketHolder.SetAckMask(ackMask);
	}

	BufferStream UDPClientSocket::GetPingPacket(void)
	{
		return OutgoingUDPPacket::CreatePingBufferStream(m_OutgoingReliablePacketHolder, m_IncomingReliablePacketHolder, m_OutgoingNonReliablePacketHolder, m_IncomingNonReliablePacketHolder);
	}

	void UDPClientSocket::SendPacket(OutgoingUDPPacket* Packet)
	{
		for (uint16_t i = 0; i < Packet->GetSliceCount(); ++i)
			SendInternal(Packet->GetSliceBuffer(i));
	}

	void UDPClientSocket::ProcessIncomingReliablePackets(void)
	{
		IncomingUDPPacketsHolder::ProcessReliablePackets(m_IncomingReliablePacketHolder, std::bind(&UDPClientSocket::HandleReceivedBuffer, this, std::placeholders::_1));
	}

	void UDPClientSocket::ProcessIncomingNonReliablePacket(IncomingUDPPacket Packet)
	{
		IncomingUDPPacketsHolder::ProcessNonReliablePacket(m_IncomingNonReliablePacketHolder, Packet, std::bind(&UDPClientSocket::HandleReceivedBuffer, this, std::placeholders::_1));
	}

	void UDPClientSocket::ProcessOutgoingReliablePackets(void)
	{
		OutgoingUDPPacketsHolder::ProcessReliablePackets(m_OutgoingReliablePacketHolder, std::bind(&UDPClientSocket::SendPacket, this, std::placeholders::_1));
	}

	void UDPClientSocket::ProcessOutgoingNonReliablePackets(void)
	{
		OutgoingUDPPacketsHolder::ProcessNonReliablePackets(m_OutgoingNonReliablePacketHolder, std::bind(&UDPClientSocket::SendPacket, this, std::placeholders::_1));
	}
}