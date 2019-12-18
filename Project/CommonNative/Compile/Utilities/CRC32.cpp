// Copyright 2019. All Rights Reserved.
#include "..\..\Include\Utilities\CRC32.h"

namespace GameFramework::Common::Utilities
{
	CRC32::CRC32(void)
	{
		InitializeTable();
	}

	uint32_t CRC32::Calculate(const std::byte* const Data, uint32_t Count)
	{
		unsigned int crc32 = 0xffffffff;

		for (unsigned int i = 0; i < Count; ++i)
		{
			unsigned int index = (crc32 ^ (char)Data[i]) & 0xFF;
			crc32 = (crc32 >> 8) ^ m_Table[index];
		}

		crc32 = crc32 ^ 0xFFFFFFFF;

		return crc32;
	}

	void CRC32::InitializeTable(void)
	{
		const int POLYNOMIAL = 0x04C11DB7;
		const unsigned int TABLE_COUNT = 256;

		unsigned int crc;
		m_Table = new unsigned int[TABLE_COUNT];

		for (int i = 0; i < TABLE_COUNT; ++i)
		{
			crc = (unsigned int)i;
			for (int j = 8; j > 0; --j)
			{
				if ((crc & 1) == 1)
					crc = (crc >> 1) ^ POLYNOMIAL;
				else
					crc >>= 1;
			}

			m_Table[i] = crc;
		}
	}

	uint32_t CRC32::CalculateHash(const std::byte* const Data, uint32_t Count)
	{
		static CRC32 crc;

		return crc.Calculate(Data, Count);
	}
}