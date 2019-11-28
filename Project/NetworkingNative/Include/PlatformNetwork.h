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
			None,
			DontRoute,
			OutOfBand
		};

		enum class ReceiveModes
		{
			None,
			DontRoute,
			OutOfBand,
			Peek
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

			BSPState,

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

		typedef uint32_t Handle;

		static bool Initialize(void);
		static bool Shutdown(void);

		static Handle Create(AddressFamilies AddressFamily, Types Type, IPProtocols IPProtocol);
		static bool Shutdown(Handle Handle, ShutdownHows How);
		static bool Close(Handle Handle);

		static bool Bind(Handle Handle, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port);

		static bool SetSocketOption(Handle Handle, OptionLevels Level, Options Option, bool Enabled);
		static bool SetSocketOption(Handle Handle, OptionLevels Level, Options Option, int32_t Value);

		static bool SetNonBlocking(Handle Handle, bool Enabled);

		static bool Send(Handle Handle, const std::byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port, SendModes Mode);

		static uint64_t AvailableBytes(Handle Handle);
		static bool Receive(Handle Handle, std::byte* Buffer, uint32_t Length, uint32_t& ReceivedLength, AddressFamilies AddressFamily, std::string& Address, uint16_t& Port, ReceiveModes Mode);

		static void ResolveDomain(const std::string& Domain, AddressFamilies& AddressFamily, std::string& Address);

		static Errors GetLastError(void);
	};
}

#endif