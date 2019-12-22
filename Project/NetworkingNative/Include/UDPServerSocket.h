// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef UDP_SERVER_SOCKET_H
#define UDP_SERVER_SOCKET_H

#include "ServerSocket.h"
#include "Packet.h"

namespace GameFramework::Networking
{
	class NETWORKING_API UDPServerSocket : public ServerSocket
	{
	private:
		class UDPClient : public Client
		{
		public:
			UDPClient(IPEndPoint EndPoint)
			{
				m_EndPoint = EndPoint;
			}

			const IPEndPoint& GetEndPoint(void) const override
			{
				return m_EndPoint;
			}

			bool GetIsConnected(void) const
			{
				return m_IsConnected;
			}

			void SetIsConnected(bool Value)
			{
				m_IsConnected = Value;
			}

			uint32_t GetMTU(void) const
			{
				return m_MTU;
			}

			void SetMTU(uint32_t Value)
			{
				m_MTU = Value;
			}

			IncomingUDPPacketsHolder& GetIncomingReliablePacketHolder(void)
			{
				return m_IncomingReliablePacketHolder;
			}

			IncomingUDPPacketsHolder& GetIncomingNonReliablePacketHolder(void)
			{
				return m_IncomingNonReliablePacketHolder;
			}

			OutgoingUDPPacketsHolder& GetOutgoingReliablePacketHolder(void)
			{
				return m_OutgoingReliablePacketHolder;
			}

			OutgoingUDPPacketsHolder& GetOutgoingNonReliablePacketHolder(void)
			{
				return m_OutgoingNonReliablePacketHolder;
			}

		private:
			bool m_IsConnected;
			IPEndPoint m_EndPoint;

			uint32_t m_MTU;

			IncomingUDPPacketsHolder m_IncomingReliablePacketHolder;
			IncomingUDPPacketsHolder m_IncomingNonReliablePacketHolder;
			OutgoingUDPPacketsHolder m_OutgoingReliablePacketHolder;
			OutgoingUDPPacketsHolder m_OutgoingNonReliablePacketHolder;
		};

		typedef List<UDPClient*> UDPClientList;
		typedef map<int32_t, UDPClient*> ClientMap;

	public:
		UDPServerSocket(void);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Length, bool Reliable = true);

		virtual void Send(const Client* Target, byte* const Buffer, uint32_t Index, uint32_t Length, bool Reliable = true);

	protected:
		virtual void SendInternal(Client* Client, BufferStream& Buffer);

		virtual void AcceptClients(void) override;

		virtual void ReadFromClients(void) override;

		void HandleIncomingBuffer(Client* Client, BufferStream& Buffer);

		bool HandleSendCommand(SendCommand* Command) override;

		void ProcessReceivedBuffer(Client* Sender, BufferStream& Buffer) override;

	private:
		void ReadFromSocket(void);

		void CheckClientsConnection(void);

		void SendPacket(Client* Client, OutgoingUDPPacket* Packet);

		void ProcessIncomingReliablePackets(UDPClient* Sender);
		void ProcessIncomingNonReliablePacket(UDPClient* Sender, IncomingUDPPacket& Packet);
		void ProcessOutgoingReliablePackets(UDPClient* Target);
		void ProcessOutgoingNonReliablePackets(UDPClient* Target);

		UDPClient* GetOrAddClient(const IPEndPoint& EndPoint);

		static uint32_t GetIPEndPointHash(const IPEndPoint& EndPoint);

	public:
		const Client* GetClients(void) const override
		{
			return reinterpret_cast<const Client*>(m_Clients.data());
		}

		uint32_t GetClientCount(void) const override
		{
			return m_Clients.size();
		}

	private:
		UDPClientList m_Clients;
		ClientMap m_ClientsMap;
	};
}

#endif