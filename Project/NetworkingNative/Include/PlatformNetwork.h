// Copyright 2016-2017 ?????????????. All Rights Reserved.
#pragma once
#ifndef PLATFORM_NETWORK_H
#define PLATFORM_NETWORK_H

#include "Common.h"
#include <utility>
#include <cstdint>
#include <cstddef>

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

		enum class InterfaceAddresses
		{
			Any,
			LoopBack,
			Broadcast,
			None
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
			Receive = 0,
			Send = 1,
			Both = 2
		};

		typedef uint32_t Handle;
		typedef uint32_t IP;

		static bool Initialize(void);
		static bool Shutdown(void);

		static Handle Create(AddressFamilies AddressFamily, Types Type, IPProtocols IPProtocol);
		static bool Shutdown(Handle Handle, ShutdownHows How);
		static bool Close(Handle Handle);

		static bool Bind(Handle Handle, AddressFamilies AddressFamily, InterfaceAddresses InterfaceAddress, uint16_t Port);

		static bool SetSocketOption(Handle Handle, bool Enabled);

		static bool SetNonBlocking(Handle Handle, bool Enabled);

		static bool Send(Handle Handle, const byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, InterfaceAddresses InterfaceAddress, uint16_t Port, SendModes Mode);
		static bool Send(Handle Handle, const byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, IP Address, uint16_t Port, SendModes Mode);

		static bool Receive(Handle Handle, byte* Buffer, uint32_t Length, uint32_t& ReceivedLength, IP& Address, uint16_t& Port, ReceiveModes Mode);

		static Errors GetLastError(void);

		static IP GetIP(uint8_t A, uint8_t B, uint8_t C, uint8_t D)
		{
			return ((A << 24) | (B << 16) | (C << 8) | D);
		}

		static void GetAddress(IP IP, uint8_t& A, uint8_t& B, uint8_t& C, uint8_t& D)
		{
			A = IP >> 24;
			B = IP >> 16;
			C = IP >> 8;
			D = IP;
		}
	};
}

#endif