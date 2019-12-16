// Copyright 2019. All Rights Reserved.
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	const uint16_t Constants::Control::SIZE = sizeof(std::byte);

	const std::byte Constants::Control::BUFFER = (std::byte)1;
	const std::byte Constants::Control::HANDSHAKE = (std::byte)2;
	const std::byte Constants::Control::HANDSHAKE_BACK = (std::byte)2;
	const std::byte Constants::Control::PING = (std::byte)3;

	const uint16_t Constants::UDP::LAST_ACK_ID_SIZE = sizeof(uint64_t);
	const uint16_t Constants::UDP::ACK_MASK_SIZE = sizeof(uint32_t);
	const uint16_t Constants::UDP::IS_RELIABLE_SIZE = sizeof(bool);
	const uint16_t Constants::UDP::ID_SIZE = sizeof(uint64_t);
	const uint16_t Constants::UDP::SLICE_COUNT_SIZE = sizeof(uint16_t);
	const uint16_t Constants::UDP::SLICE_INDEX_SIZE = sizeof(uint16_t);
	const uint16_t Constants::UDP::PACKET_HEADER_SIZE = Constants::UDP::LAST_ACK_ID_SIZE + Constants::UDP::ACK_MASK_SIZE + Constants::UDP::IS_RELIABLE_SIZE + Constants::UDP::ID_SIZE + Constants::UDP::SLICE_COUNT_SIZE + Constants::UDP::SLICE_INDEX_SIZE;

	const uint32_t Constants::RECEIVE_TIMEOUT = 1;
	const uint32_t Constants::SEND_TIMEOUT = 1;
	const uint32_t Constants::RECEIVE_BUFFER_SIZE = 8 * 1024;
	const uint32_t Constants::SEND_BUFFER_SIZE = 8 * 1024;
	const uint16_t Constants::TIME_TO_LIVE = 64;
	const float Constants::PING_TIME = 5;

	const uint32_t Constants::UDP_MAX_MTU = 1500;

	const uint32_t Constants::DEFAULT_PACKET_COUNT_RATE = 32;

	GameFramework::Common::Utilities::Random Constants::Random;
}