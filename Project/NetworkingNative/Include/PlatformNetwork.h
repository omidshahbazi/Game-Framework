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
			UDP,
			ICMP
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
			InsufficientBuffer,
			InvalidParameters,
			IOPending,
			NotEnoughMemory,
			NotSupported
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

		enum class PingStatus
		{
			Unknown,
			Success,
			DestinationNetworkUnreachable,
			DestinationHostUnreachable,
			DestinationProtocolUnreachable,
			DestinationPortUnreachable,
			NoResources,
			BadOption,
			HardwareError,
			PacketTooBig,
			TimedOut,
			BadRoute,
			TTLExpired,
			TTLReassemblyTimeExceeded,
			ParameterProblem,
			SourceQuench,
			BadDestination,
			DestinationUnreachable,
			TimeExceeded,
			BadHeader,
			UnrecognizedNextHeader,
			ICMPError,
			DestinationScopeMismatch
		};

		class SocketException : public std::exception
		{
		public:
			SocketException(Errors Error) :
				m_Error(Error)
			{
			}

			SocketException(const std::string& Message) :
				m_Error(Errors::NoError),
				m_Message(Message)
			{
			}

			Errors GetError(void) const
			{
				return m_Error;
			}

			const std::string& GetMessage(void) const
			{
				return m_Message;
			}

		private:
			Errors m_Error;
			std::string m_Message;
		};

		struct PingOptions
		{
		public:
			PingOptions(void) :
				TTL(128),
				Reverse(false),
				DontFragment(false)
			{
			}

		public:
			uint16_t TTL;
			bool Reverse;
			bool DontFragment;
		};

		struct PingReply
		{
		public:
			PingReply() :
				Status(PingStatus::Unknown),
				RoundTripTime(0)
			{
			}

			PingReply(PingStatus Status, uint64_t RoundTripTime) :
				Status(Status),
				RoundTripTime(RoundTripTime)
			{
			}

		public:
			PingStatus Status;
			uint64_t RoundTripTime;
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

		static bool Connect(Handle Handle, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port);

		static uint32_t Send(Handle Handle, const std::byte* Buffer, uint32_t Length, SendModes Mode);
		//static bool SendTo(Handle Handle, const std::byte* Buffer, uint32_t Length, AddressFamilies AddressFamily, const std::string& Address, uint16_t Port, SendModes Mode);

		static bool Poll(Handle Handle, uint32_t Timeout, SelectModes Mode);

		static bool Select(Handle Handle, SelectModes Mode, uint32_t Timeout);

		static uint64_t GetAvailableBytes(Handle Handle);
		static bool Receive(Handle Handle, std::byte* Buffer, uint32_t& Length, ReceiveModes Mode);
		//static bool ReceiveFromm(Handle Handle, std::byte* Buffer, uint32_t Length, uint32_t& ReceivedLength, AddressFamilies AddressFamily, std::string& Address, uint16_t& Port, ReceiveModes Mode);

		static void ResolveDomain(const std::string& Domain, AddressFamilies& AddressFamily, std::string& Address);

		static Errors GetLastError(void);

		static PingReply Ping(AddressFamilies AddressFamily, const std::string& Address, uint32_t Timeout, std::byte* Buffer, uint32_t BufferLength, const PingOptions& Options);
	};
}

#endif