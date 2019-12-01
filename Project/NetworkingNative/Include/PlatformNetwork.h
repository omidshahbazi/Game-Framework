// Copyright 2019. All Rights Reserved.
#pragma once
#ifndef PLATFORM_NETWORK_H
#define PLATFORM_NETWORK_H

#include "Common.h"
#include <cstddef>
#include <inttypes.h>
#include <string>

namespace GameFramework::Networking
{
	static class NETWORKING_API PlatformNetwork
	{
	public:
		enum class AddressFamilies
		{
			InterNetwork,
			InterNetworkV6
		};

		enum class Types
		{
			Stream,
			Datagram,
			RawProtocol,
			ReliablyDeliverdMessage,
			SequenedPacketStream
		};

		enum class IPProtocols
		{
			TCP,
			UDP
		};

		enum class SendModes
		{
			None = 0,
			DontRoute = 2,
			OutOfBand = 4
		};

		enum class ReceiveModes
		{
			None = 0,
			DontRoute = 2,
			OutOfBand = 4,
			Peek = 8
		};

		enum class SelectModes
		{
			SelectRead = 0,
			SelectWrite = 1,
			SelectError = 2
		};

		enum class Errors
		{
			NoError,
			BaseError,
			Interrupted,
			BadFile,
			AccessDenied,
			InvalidPointer,
			InvalidArguments,
			TooManySockets,
			WouldBlock,
			BlockingInProgress,
			NonBlockingInProgress,
			NoSocket,
			DestinationAddressRequired,
			LargePacketSize,
			MismatchProtocolType,
			NoProtocolOperation,
			ProtocolNotSupported,
			SocketNotSupported,
			OperationNotSupported,
			ProtocolFamilyNotSupported,
			AddressFamilyNotSupported,
			AddressInUse,
			AddressNotValid,
			NetworkDown,
			NetworkUnreachable,
			NetworkReset,
			ConnectionAbort,
			ConnectionReset,
			NoBuffer,
			IsConnected,
			NotConnected,
			Shutdown,
			TooManyReferences,
			Timeout,
			ConnectionRefused,
			Loop,
			NameTooLong,
			HostDown,
			HostUnreachable,
			NotEmpty,
			ProcessLimit,
			OutOfUsers,
			OutOfDisk,
			HandleNotExists,
			ItemNotExists,
			SystemNotReady,
			VersionNotSupported,
			NotInitialized,
			Disconnected,
			InvalidProcedureTable,
			InvalidProvider,
			ProviderFailedToInitialize,
			SystemCallFailure,
			ServiceNotFound,
			TypeNotFound,
			NoMoreResult2,
			Canceled2,
			Refused,
			HostNotFound,
			TryAgain,
			NoRecoverable,
			NoData,
			QOSReceivers,
			QOSNoSenders,
			QOSNoReceiver,
			QOSRequestConfirmed,
			AdmissionFailure,
			QOSPolicyFailure,
			QOSBadStyle,
			QOSBadObject,
			QOSTrafficControlError,
			QOSGenericError,
			QOSServiceType,
			QOSFlowSpecific,
			QOSProviderSpecific,
			QOSFilterStyle,
			QOSFilterType,
			QOSFilterCount,
			QOSObjectLength,
			QOSLowCount,
			QOSUnknownProviderSpecific,
			QOSPolicyObject,
			QOSFlowDescriptor,
			QOSInconsistentFlowSpecific,
			QOSInconsistentFilterSpecific,
			QOSShapeDiscardMode,
			QOSShapingRateObject,
			QOSReceivedProviderType,
			QOSSecureHostNotFound,
			IPSecPolicy,
		};

		enum class ShutdownHows
		{
			Receive,
			Send,
			Both
		};

		enum class OptionLevels
		{
			IP = 0,
			TCP = 6,
			UDP = 17,
			IPV6 = 41,
			Socket = 65535
		};

		enum class Options
		{
			Debug,
			AcceptConnection,
			ReuseAddress,
			KeepAlive,
			DontRoute,
			Broadcast,
			UseLoopback,
			Linger,

			DontLinger,

			SendBuffer,
			ReceiveBuffer,

			SendTimeout,
			ReceiveTimeout,

			GroupID,
			GroupPriority,

			MaxMessageSize,

			ConditionalAccept,

			PauseAccept,
			RandomizePort,
			PortScalability,
			ReuseUnicastPort,

			ReuseMulticastPort,

			NoDelay,
			TimeToLive,
			IPv6Only,
			Checksum
		};

		class SocketException : public std::exception
		{
		public:
			SocketException(Errors Error) :
				m_Error(Error)
			{
			}

			Errors GetError(void) const
			{
				return m_Error;
			}

		private:
			Errors m_Error;
		};

		typedef uint32_t Handle;

		static void Initialize(void);
		static void Shutdown(void);

		static Handle Create(AddressFamilies AddressFamily, Types Type, IPProtocols IPProtocol);
		static void Shutdown(Handle Handle, ShutdownHows How);
		static void Close(Handle Handle);

		static void SetSocketOption(Handle Handle, OptionLevels Level, Options Option, bool Enabled);
		static void SetSocketOption(Handle Handle, OptionLevels Level, Options Option, int32_t Value);
		static void SetBlocking(Handle Handle, bool Enabled);

		static void Bind(Handle Handle, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port);

		static void Listen(Handle Handle, uint32_t MaxConnections);

		static bool Accept(Handle ListenerHandle, Handle& AcceptedHandle, AddressFamilies& AddressFamily, std::string& Address, uint16_t& Port);

		static uint32_t Send(Handle Handle, const std::byte* Buffer, uint32_t Length, SendModes Mode);
		//static bool SendTo(Handle Handle, const std::byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port, SendModes Mode);

		static bool Poll(Handle Handle, uint32_t Timeout, SelectModes Mode);

		static uint64_t GetAvailableBytes(Handle Handle);
		static bool Receive(Handle Handle, std::byte* Buffer, uint32_t& Length, ReceiveModes Mode);
		//static bool ReceiveFromm(Handle Handle, std::byte* Buffer, uint32_t Length, uint32_t& ReceivedLength, AddressFamilies AddressFamily, std::string& Address, uint16_t& Port, ReceiveModes Mode);

		static void ResolveDomain(const std::string& Domain, AddressFamilies& AddressFamily, std::string& Address);

		static Errors GetLastError(void);
	};
}

#endif