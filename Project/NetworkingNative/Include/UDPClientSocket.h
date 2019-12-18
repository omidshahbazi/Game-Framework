// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_CLIENT_SOCKET_H
#define TCP_CLIENT_SOCKET_H

#include "ClientSocket.h"
#include "Packet.h"

namespace GameFramework::Networking
{
	//Unexpected disconnection doesn't work
	//Memory leak still remains
	class NETWORKING_API UDPClientSocket : public ClientSocket
	{
	public:
		UDPClientSocket(void);

		virtual void Send(byte* const Buffer, uint32_t Length, bool Reliable = true);

		virtual void Send(byte* const Buffer, uint32_t Index, uint32_t Length, bool Reliable = true);

	protected:
		virtual void SendInternal(BufferStream& Buffer);

		void ConnectInternal(const IPEndPoint& EndPoint) override;

		virtual void HandleIncomingBuffer(BufferStream& Buffer) override;

		void ProcessReceivedBuffer(BufferStream& Buffer) override;

		virtual void HandlePingPacketPayload(BufferStream& Buffer) override;

		virtual BufferStream GetPingPacket(void) override;

		uint32_t GetMTU(void) const
		{
			return m_MTU;
		}

	private:
		void SendPacket(OutgoingUDPPacket* Packet);

		void ProcessIncomingReliablePackets(void);
		void ProcessIncomingNonReliablePacket(IncomingUDPPacket Packet);
		void ProcessOutgoingReliablePackets(void);
		void ProcessOutgoingNonReliablePackets(void);

	private:
		uint32_t m_MTU;

		IncomingUDPPacketsHolder m_IncomingReliablePacketHolder;
		IncomingUDPPacketsHolder m_IncomingNonReliablePacketHolder;
		OutgoingUDPPacketsHolder m_OutgoingReliablePacketHolder;
		OutgoingUDPPacketsHolder m_OutgoingNonReliablePacketHolder;
	};
}

#endif