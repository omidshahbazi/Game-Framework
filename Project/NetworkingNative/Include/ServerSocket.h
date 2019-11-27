// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TCP_SERVER_SOCKET_H
#define TCP_SERVER_SOCKET_H

#include "BaseSocket.h"

namespace GameFramework::Networking
{
	class NETWORKING_API ServerSocket : public BaseSocket
	{
	public:
		ServerSocket(PlatformNetwork::IPProtocols Type, uint32_t MaxConnection);

		void Bind(const std::string& Host, uint16_t Port);

		void Bind(const IPAddress& IP, uint16_t Port);

		void Bind(const IPEndPoint& EndPoint);

	protected:
		virtual void Receive(void) override;

		virtual bool HandleSendCommand(const SendCommand& Command) override;

		virtual void ProcessEvent(const EventBase& Event)  override;

	protected:
		virtual bool GetIsReady(void) override;

		virtual double GetTimestamp(void)  override;
	};
}

#endif