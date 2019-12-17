// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef PACKET_H
#define PACKET_H

#include "Common.h"
#include <cstddef>
#include <stdint.h>
#include <BufferStream.h>
#include <map>

using namespace GameFramework::BinarySerializer;

namespace GameFramework::Networking
{
	static class NETWORKING_API Packet
	{
	public:
		static const uint16_t PACKET_SIZE_SIZE;
		static const uint16_t HEADER_SIZE;

		static const uint16_t PING_SIZE;

	public:
		static BufferStream CreateOutgoingBufferStream(uint32_t Length);

		static BufferStream CreateIncomingBufferStream(std::byte* const Buffer, uint32_t Length);

		static BufferStream CreateHandshakeBufferStream(uint32_t MTU);

		static BufferStream CreateHandshakeBackBufferStream(uint32_t PacketCountRate);

		static BufferStream CreatePingBufferStream(uint32_t PayloadSize = 0);
	};

	class UDPPacket
	{
	public:
		UDPPacket(uint64_t ID, uint32_t SliceCount) :
			m_ID(ID),
			m_SliceCount(m_SliceCount),
			m_SliceBuffers(nullptr)
		{
			uint32_t size = m_SliceCount * sizeof(BufferStream*);
			m_SliceBuffers = reinterpret_cast<BufferStream * *>(malloc(size));
			memset(m_SliceBuffers, 0, size);
		}

		~UDPPacket(void)
		{
			free(m_SliceBuffers);
		}

		uint64_t GetID(void) const
		{
			return m_ID;
		}

		BufferStream* GetSliceBuffer(uint32_t Index)
		{
			return m_SliceBuffers[Index];
		}

		void SetScliceBuffer(uint32_t Index, BufferStream* Buffer)
		{
			if (m_SliceBuffers[Index] != nullptr)
				return;

			m_SliceBuffers[Index] = Buffer;
		}

		uint32_t GetSliceCount(void) const
		{
			return m_SliceCount;
		}

		uint32_t GetLength(void) const
		{
			uint32_t length = 0;

			for (uint32_t i = 0; i < m_SliceCount; ++i)
			{
				BufferStream* buffer = m_SliceBuffers[i];

				if (buffer == nullptr)
					continue;

				length += buffer->GetSize();
			}

			return length;
		}

	private:
		uint64_t m_ID;
		BufferStream** m_SliceBuffers;
		uint32_t m_SliceCount;
	};

	template<class T>
	class UDPPacketsHolder
	{
	public:
		typedef map<uint64_t, T*> PacketMap;

	public:
		UDPPacketsHolder(void) :
			m_LastID(0)
		{
		}

		public T* GetPacket(uint64_t ID)
		{
			if (m_PacketsMap.find(ID) != m_PacketsMap.end())
				return m_PacketsMap[ID];

			return nullptr;
		}

		public void AddPacket(T* Packet)
		{
			m_PacketsMap[Packet->GetID()] = Packet;
		}

		PacketMap& GetPacketsMap(void)
		{
			return m_PacketsMap;
		}

		uint64_t GetLastID(void) const
		{
			return m_LastID;
		}

	protected:
		void SetLastID(uint64_t LastID)
		{
			m_LastID = LastID;
		}

	private:
		PacketMap m_PacketsMap;
		uint64_t m_LastID;
	};

	class IncomingUDPPacket : public UDPPacket
	{
	public:
		IncomingUDPPacket(uint64_t ID, uint32_t SliceCount) :
			UDPPacket(ID, SliceCount)
		{
		}

		BufferStream Combine(void)
		{
			BufferStream buffer(GetLength());

			uint32_t count = GetSliceCount();

			for (uint32_t i = 0; i < count; ++i)
			{
				BufferStream& sliceBuffer = *GetSliceBuffer(i);
				buffer.WriteBytes(sliceBuffer.GetBuffer(), buffer.GetSize());
			}

			return buffer;
		}

		bool GetIsCompleted(void)
		{
			uint32_t count = GetSliceCount();

			for (uint32_t i = 0; i < count; ++i)
				if (GetSliceBuffer(i) == nullptr)
					return false;

			return true;
		}
	};

	class OutgoingUDPPacket : public UDPPacket
	{
	public:
		OutgoingUDPPacket(uint64_t ID, uint32_t SliceCount) :
			UDPPacket(ID, SliceCount)
		{
		}

		static OutgoingUDPPacket CreateOutgoingBufferStream(OutgoingUDPPacketsHolder& OutgoingHolder, IncomingUDPPacketsHolder& IncomingHolder, byte* const Buffer, uint32_t Index, uint32_t Length, uint32_t MTU, bool IsReliable)
		{
			if (Constants::UDP::PACKET_HEADER_SIZE >= MTU)
				throw exception(("PACKET_HEADER_SIZE [" + std::to_string(Constants::UDP::PACKET_HEADER_SIZE) + "] is greater than or equal to MTU [" + std::to_string(MTU) + "]").c_str());

			OutgoingHolder.IncreaseLastID();

			uint64_t id = OutgoingHolder.GetLastID();

			uint32_t mtu = MTU - Constants::UDP::PACKET_HEADER_SIZE;

			uint16_t sliceCount = (uint16_t)ceil(Length / (float)mtu);

			OutgoingUDPPacket packet(id, sliceCount);

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

				packet.SetSliceBuffer(i, buffer);
			}

			OutgoingHolder.AddPacket(packet);

			return packet;
		}

		static BufferStream CreatePingBufferStream(OutgoingUDPPacketsHolder& ReliableOutgoingHolder, IncomingUDPPacketsHolder& ReliableIncomingHolder, OutgoingUDPPacketsHolder& NonReliableOutgoingHolder, IncomingUDPPacketsHolder& NonReliableIncomingHolder)
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
	};

	class IncomingUDPPacketsHolder : public UDPPacketsHolder<IncomingUDPPacket>
	{
	public:
		static uint32_t GetAckMask(IncomingUDPPacketsHolder& IncomingHolder, uint32_t AckMask)
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

		static void ProcessReliablePackets(IncomingUDPPacketsHolder Holder, std::function<void(BufferStream)> HandleReceivedBuffer)
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

		static void ProcessNonReliablePacket(IncomingUDPPacketsHolder Holder, IncomingUDPPacket Packet, std::function<void(BufferStream)> HandleReceivedBuffer)
		{
			HandleReceivedBuffer(Packet.Combine());

			Holder.GetPacketsMap().erase(Packet.GetID());
		}

		void SetLastID(uint64_t Value)
		{
			UDPPacketsHolder::SetLastID(Value);
		}

		uint64_t GetPrevID(void) const
		{
			return m_PrevID;
		}

		void SetPrevID(uint64_t Value)
		{
			m_PrevID = Value;
		}

	private:
		uint64_t m_PrevID;
	};

	class OutgoingUDPPacketsHolder : UDPPacketsHolder<OutgoingUDPPacket>
	{
	public:

		static void ProcessReliablePackets(OutgoingUDPPacketsHolder Holder, std::function<void(OutgoingUDPPacket*)> SendPacket)
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

		static void ProcessNonReliablePackets(OutgoingUDPPacketsHolder Holder, std::function<void(OutgoingUDPPacket*)> SendPacket)
		{
			ProcessReliablePackets(Holder, SendPacket);
		}

		void IncreaseLastID(void)
		{
			SetLastID(GetLastID() + 1);
		}

		uint64_t GetLastAckID(void) const
		{
			return m_LastAckID;
		}

		void SetLastAckID(uint64_t Value)
		{
			m_LastAckID = Value;
		}

		uint32_t GetAckMask(void) const
		{
			return m_AckMask;
		}

		void SetAckMask(uint32_t Value)
		{
			m_AckMask = Value;
		}

	private:
		uint64_t m_LastAckID;
		uint32_t m_AckMask;
	};
}

#endif