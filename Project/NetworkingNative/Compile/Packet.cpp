// Copyright 2019. All Rights Reserved.
#include "..\Include\Packet.h"
#include "..\Include\Constants.h"
#include <Timing\Time.h>
#include <Utilities\BitwiseHelper.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	const uint16_t Packet::PACKET_SIZE_SIZE = sizeof(uint32_t);
	const uint16_t Packet::HEADER_SIZE = Constants::Control::SIZE;

	const uint16_t Packet::PING_SIZE = Packet::PACKET_SIZE_SIZE + Packet::HEADER_SIZE + sizeof(double);

	BufferStream Packet::CreateOutgoingBufferStream(uint32_t Length)
	{
		uint32_t length = HEADER_SIZE + Length;

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::BUFFER);

		return buffer;
	}

	BufferStream Packet::CreateIncomingBufferStream(std::byte* const Buffer, uint32_t Length)
	{
		return BufferStream(Buffer, HEADER_SIZE, (uint32_t)(Length - HEADER_SIZE));
	}

	BufferStream Packet::CreateHandshakeBufferStream(uint32_t MTU)
	{
		uint32_t length = HEADER_SIZE + sizeof(uint32_t);

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::HANDSHAKE);
		buffer.WriteUInt32(MTU);

		return buffer;
	}

	BufferStream Packet::CreateHandshakeBackBufferStream(uint32_t PacketCountRate)
	{
		uint32_t length = HEADER_SIZE + sizeof(uint32_t);

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::HANDSHAKE);
		buffer.WriteUInt32(PacketCountRate);

		return buffer;
	}

	BufferStream Packet::CreatePingBufferStream(uint32_t PayloadSize)
	{
		uint32_t length = (PING_SIZE - PACKET_SIZE_SIZE) + PayloadSize;

		BufferStream buffer(PACKET_SIZE_SIZE + length);
		buffer.ResetWrite();
		buffer.WriteUInt32(length);
		buffer.WriteBytes(Constants::Control::PING);
		buffer.WriteFloat64(Time::GetCurrentEpochTime());

		return buffer;
	}

	UDPPacket::UDPPacket(uint64_t ID, uint32_t SliceCount) :
		m_ID(ID),
		m_SliceCount(m_SliceCount),
		m_SliceBuffers(nullptr)
	{
		uint32_t size = m_SliceCount * sizeof(BufferStream);
		m_SliceBuffers = reinterpret_cast<BufferStream*>(malloc(size));

		for (uint32_t i = 0; i < m_SliceCount; ++i)
			new (&m_SliceBuffers[i]) BufferStream(0);
	}

	UDPPacket::	~UDPPacket(void)
	{
		free(m_SliceBuffers);
	}

	uint32_t UDPPacket::GetLength(void) const
	{
		uint32_t length = 0;

		for (uint32_t i = 0; i < m_SliceCount; ++i)
		{
			BufferStream* buffer = &m_SliceBuffers[i];

			length += buffer->GetSize();
		}

		return length;
	}

	IncomingUDPPacket::IncomingUDPPacket(uint64_t ID, uint32_t SliceCount) :
		UDPPacket(ID, SliceCount)
	{
	}

	BufferStream IncomingUDPPacket::Combine(void)
	{
		BufferStream buffer(GetLength());

		uint32_t count = GetSliceCount();

		for (uint32_t i = 0; i < count; ++i)
		{
			BufferStream& sliceBuffer = GetSliceBuffer(i);
			buffer.WriteBytes(sliceBuffer.GetBuffer(), 0, buffer.GetSize());
		}

		return buffer;
	}

	OutgoingUDPPacket::OutgoingUDPPacket(uint64_t ID, uint32_t SliceCount) :
		UDPPacket(ID, SliceCount)
	{
	}

	OutgoingUDPPacket* OutgoingUDPPacket::CreateOutgoingBufferStream(OutgoingUDPPacketsHolder& OutgoingHolder, IncomingUDPPacketsHolder& IncomingHolder, byte* const Buffer, uint32_t Index, uint32_t Length, uint32_t MTU, bool IsReliable)
	{
		if (Constants::UDP::PACKET_HEADER_SIZE >= MTU)
			throw exception(("PACKET_HEADER_SIZE [" + std::to_string(Constants::UDP::PACKET_HEADER_SIZE) + "] is greater than or equal to MTU [" + std::to_string(MTU) + "]").c_str());

		OutgoingHolder.IncreaseLastID();

		uint64_t id = OutgoingHolder.GetLastID();

		uint32_t mtu = MTU - Constants::UDP::PACKET_HEADER_SIZE;

		uint16_t sliceCount = (uint16_t)ceil(Length / (float)mtu);

		OutgoingUDPPacket* packet = new OutgoingUDPPacket(id, sliceCount);

		uint32_t ackMask = IncomingUDPPacketsHolder::GetAckMask(IncomingHolder, OutgoingHolder.GetAckMask());

		for (uint16_t i = 0; i < sliceCount; ++i)
		{
			uint32_t index = Index + (i * mtu);
			uint32_t length = fmin(mtu, Length - (i * mtu));

			BufferStream buffer = Packet::CreateOutgoingBufferStream(Constants::UDP::PACKET_HEADER_SIZE + length);
			buffer.WriteUInt64(IncomingHolder.GetLastID());
			buffer.WriteUInt32(ackMask);
			buffer.WriteBool(IsReliable);
			buffer.WriteUInt64(id);
			buffer.WriteUInt16(sliceCount);
			buffer.WriteUInt16(i);
			buffer.WriteBytes(Buffer, index, length);

			packet->SetSliceBuffer(i, buffer);
		}

		OutgoingHolder.AddPacket(packet);

		return packet;
	}

	BufferStream OutgoingUDPPacket::CreatePingBufferStream(OutgoingUDPPacketsHolder& ReliableOutgoingHolder, IncomingUDPPacketsHolder& ReliableIncomingHolder, OutgoingUDPPacketsHolder& NonReliableOutgoingHolder, IncomingUDPPacketsHolder& NonReliableIncomingHolder)
	{
		BufferStream buffer = Packet::CreatePingBufferStream((Constants::UDP::LAST_ACK_ID_SIZE + Constants::UDP::ACK_MASK_SIZE) * 2);

		uint32_t ackMask = IncomingUDPPacketsHolder::GetAckMask(ReliableIncomingHolder, ReliableOutgoingHolder.GetAckMask());
		buffer.WriteUInt64(ReliableIncomingHolder.GetLastID());
		buffer.WriteUInt32(ackMask);

		ackMask = IncomingUDPPacketsHolder::GetAckMask(NonReliableIncomingHolder, NonReliableOutgoingHolder.GetAckMask());
		buffer.WriteUInt64(NonReliableIncomingHolder.GetLastID());
		buffer.WriteUInt32(ackMask);

		return buffer;
	}

	uint32_t IncomingUDPPacketsHolder::GetAckMask(IncomingUDPPacketsHolder& IncomingHolder, uint32_t AckMask)
	{
		AckMask <<= 1;

		uint16_t bitCount = Constants::UDP::ACK_MASK_SIZE * 8;

		for (uint16_t i = 0; i < bitCount; ++i)
		{
			uint16_t offset = i + 1;

			if (offset >= IncomingHolder.GetLastID())
				break;

			uint64_t packetID = IncomingHolder.GetLastID() - offset;

			IncomingUDPPacket* packet = IncomingHolder.GetPacketsMap()[packetID];

			if (packet == nullptr || packet->GetIsCompleted())
				AckMask = BitwiseHelper::Enable(AckMask, bitCount - offset);
		}

		return AckMask;
	}

	void IncomingUDPPacketsHolder::ProcessReliablePackets(IncomingUDPPacketsHolder Holder, HandleReceivedBufferCallback HandleReceivedBuffer)
	{
		List<uint64_t> completedIDs;

		auto& packetMap = Holder.GetPacketsMap();
		auto it = packetMap.begin();
		auto end = packetMap.end();
		for (; it != end; ++it)
		{
			uint64_t id = (*it).first;
			IncomingUDPPacket* packet = (*it).second;

			if (!packet->GetIsCompleted())
				break;

			if (id < Holder.GetPrevID())
			{
				completedIDs.push_back(id);
				continue;
			}

			if (id - Holder.GetPrevID() > 1)
				break;

			HandleReceivedBuffer(packet->Combine());

			Holder.SetPrevID(id);

			completedIDs.push_back(id);
		}

		for (uint32_t i = 0; i < completedIDs.size(); ++i)
			Holder.GetPacketsMap().erase(completedIDs[i]);
	}

	void IncomingUDPPacketsHolder::ProcessNonReliablePacket(IncomingUDPPacketsHolder Holder, IncomingUDPPacket Packet, HandleReceivedBufferCallback HandleReceivedBuffer)
	{
		HandleReceivedBuffer(Packet.Combine());

		Holder.GetPacketsMap().erase(Packet.GetID());
	}

	void OutgoingUDPPacketsHolder::ProcessReliablePackets(OutgoingUDPPacketsHolder Holder, SendPacketCallback SendPacket)
	{
		uint64_t lastAckID = Holder.GetLastAckID();

		if (lastAckID == 0)
			return;

		uint16_t bitCount = Constants::UDP::ACK_MASK_SIZE * 8;

		uint16_t count = fmin(lastAckID - 1, bitCount);

		for (short i = (short)count; i >= 0; --i)
		{
			uint64_t id = lastAckID - i;

			bool acked = (BitwiseHelper::IsEnabled(Holder.GetAckMask(), bitCount - i) || id == lastAckID);

			if (acked)
			{
				Holder.GetPacketsMap().erase(id);
				continue;
			}

			OutgoingUDPPacket* packet = Holder.GetPacket(id);

			SendPacket(packet);
		}
	}

	void OutgoingUDPPacketsHolder::ProcessNonReliablePackets(OutgoingUDPPacketsHolder Holder, SendPacketCallback SendPacket)
	{
		ProcessReliablePackets(Holder, SendPacket);
	}
}