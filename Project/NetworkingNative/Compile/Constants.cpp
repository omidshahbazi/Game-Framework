// Copyright 2019. All Rights Reserved.
#include "..\Include\Constants.h"
#include <Timing\Time.h>

using namespace GameFramework::Common::Timing;
using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	const int Constants::Control::SIZE = sizeof(std::byte);

	const std::byte Constants::Control::BUFFER = (std::byte)1;
	const std::byte Constants::Control::PING = (std::byte)2;

	const uint32_t Constants::RECEIVE_TIMEOUT = 1;
	const uint32_t Constants::SEND_TIMEOUT = 1;
	const uint32_t Constants::RECEIVE_BUFFER_SIZE = 8 * 1024;
	const uint32_t Constants::SEND_BUFFER_SIZE = 8 * 1024;
	const uint16_t Constants::TIME_TO_LIVE = 64;
	const float Constants::PING_TIME = 5;

	GameFramework::Common::Utilities::Random Constants::Random;
}