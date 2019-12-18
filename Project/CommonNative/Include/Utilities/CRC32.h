// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CRC32_H
#define CRC32_H

#include "..\Common.h"
#include <stdint.h>
#include <cstddef>

namespace GameFramework::Common::Utilities
{
	class COMMON_API CRC32
	{
	private:
		CRC32(void);

		uint32_t Calculate(const std::byte* const Data, uint32_t Count);

		void InitializeTable(void);

	public:
		static uint32_t CalculateHash(const std::byte* const Data, uint32_t Count);

	private:
		uint32_t* m_Table;
	};
}

#endif