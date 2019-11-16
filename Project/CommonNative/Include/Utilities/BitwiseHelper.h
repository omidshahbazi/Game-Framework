// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BITWISE_HELPER_H
#define BITWISE_HELPER_H

#include "..\Common.h"

namespace GameFramework::Common::Utilities
{
	static class COMMON_API BitwiseHelper
	{
	public:
		static int32_t Enable(int32_t Mask, int32_t Bit)
		{
			return (Mask | Bit);
		}
		static long Enable(int64_t Mask, int64_t Bit)
		{
			return (Mask | Bit);
		}

		static int32_t Disable(int32_t Mask, int32_t Bit)
		{
			return Mask ^ (Mask & Bit);
		}
		static long Disable(int64_t Mask, int64_t Bit)
		{
			return Mask ^ (Mask & Bit);
		}

		static int32_t Toggle(int32_t Mask, int32_t Bit)
		{
			return (Mask ^ Bit);
		}
		static long Toggle(int64_t Mask, int64_t Bit)
		{
			return (Mask ^ Bit);
		}

		static bool IsEnabled(int32_t Mask, int32_t Bit)
		{
			return (Mask == Bit || (Mask & Bit) != 0);
		}
		static bool IsEnabled(int64_t Mask, int64_t Bit)
		{
			return (Mask == Bit || (Mask & Bit) != 0);
		}
	};
}

#endif