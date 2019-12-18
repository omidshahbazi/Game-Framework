// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef PACKET_H
#define PACKET_H

#include "Common.h"
#include <cstddef>
#include <stdint.h>
#include <BufferStream.h>
#include <map>
#include  <functional>

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
		UDPPacket(uint64_t ID, uint32_t SliceCount);

		~UDPPacket(void);

		uint64_t GetID(void) const
		{
			return m_ID;
		}

		BufferStream& GetSliceBuffer(uint32_t Index)
		{
			return m_SliceBuffers[Index];
		}

		void SetSliceBuffer(uint32_t Index, BufferStream& Buffer)
		{
			if (m_SliceBuffers[Index].GetSize() != 0)
				return;

			m_SliceBuffers[Index] = Buffer;
		}

		uint32_t GetSliceCount(void) const
		{
			return m_SliceCount;
		}

		uint32_t GetLength(void) const;

	private:
		uint64_t m_ID;
		BufferStream* m_SliceBuffers;
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

		T* GetPacket(uint64_t ID)
		{
			if (m_PacketsMap.find(ID) != m_PacketsMap.end())
				return m_PacketsMap[ID];

			return nullptr;
		}

		void AddPacket(T* Packet)
		{
			m_PacketsMap[Packet->GetID()] = Packet;
		}

		void RemovePacket(uint64_t ID)
		{
			m_PacketsMap.erase(ID);
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
		IncomingUDPPacket(uint64_t ID, uint32_t SliceCount);

		BufferStream Combine(void);

		bool GetIsCompleted(void)
		{
			uint32_t count = GetSliceCount();

			for (uint32_t i = 0; i < count; ++i)
				if (GetSliceBuffer(i).GetSize() == 0)
					return false;

			return true;
		}
	};

	class OutgoingUDPPacketsHolder;
	class IncomingUDPPacketsHolder;

	class OutgoingUDPPacket : public UDPPacket
	{
	public:
		OutgoingUDPPacket(uint64_t ID, uint32_t SliceCount);

		static OutgoingUDPPacket* CreateOutgoingBufferStream(OutgoingUDPPacketsHolder& OutgoingHolder, IncomingUDPPacketsHolder& IncomingHolder, byte* const Buffer, uint32_t Index, uint32_t Length, uint32_t MTU, bool IsReliable);

		static BufferStream CreatePingBufferStream(OutgoingUDPPacketsHolder& ReliableOutgoingHolder, IncomingUDPPacketsHolder& ReliableIncomingHolder, OutgoingUDPPacketsHolder& NonReliableOutgoingHolder, IncomingUDPPacketsHolder& NonReliableIncomingHolder);
	};

	class IncomingUDPPacketsHolder : public UDPPacketsHolder<IncomingUDPPacket>
	{
	public:
		typedef std::function<void(const BufferStream&)> HandleReceivedBufferCallback;

	public:
		static uint32_t GetAckMask(IncomingUDPPacketsHolder& IncomingHolder, uint32_t AckMask);

		static void ProcessReliablePackets(IncomingUDPPacketsHolder& Holder, HandleReceivedBufferCallback HandleReceivedBuffer);

		static void ProcessNonReliablePacket(IncomingUDPPacketsHolder& Holder, IncomingUDPPacket& Packet, HandleReceivedBufferCallback HandleReceivedBuffer);

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

	class OutgoingUDPPacketsHolder : public UDPPacketsHolder<OutgoingUDPPacket>
	{
	public:
		typedef std::function<void(OutgoingUDPPacket*)> SendPacketCallback;

	public:
		static void ProcessReliablePackets(OutgoingUDPPacketsHolder& Holder, SendPacketCallback SendPacket);

		static void ProcessNonReliablePackets(OutgoingUDPPacketsHolder& Holder, SendPacketCallback SendPacket);

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