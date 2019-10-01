// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef CRC32_H
#define CRC32_H

namespace GameFramework::Common::Utilities
{
	class CRC32
	{
	private:
		CRC32(void)
		{
			InitializeTable();
		}

		unsigned int Calculate(const char * const Data, unsigned int Count)
		{
			unsigned int crc32 = 0xffffffff;

			for (unsigned int i = 0; i < Count; ++i)
			{
				unsigned int index = (crc32 ^ Data[i]) & 0xFF;
				crc32 = (crc32 >> 8) ^ m_Table[index];
			}

			crc32 = crc32 ^ 0xFFFFFFFF;

			return crc32;
		}

		void InitializeTable(void)
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

	public:
		static unsigned int CalculateHash(const char * const Data, unsigned int Count)
		{
			static CRC32 crc;

			return crc.Calculate(Data, Count);
		}

	private:
		unsigned int *m_Table;
	};
}

#endif