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
		static int32_t Enable(int32_t Mask, int32_t BitIndex)
		{
			return Enable((int64_t)Mask, (int64_t)BitIndex);
		}
		static long Enable(int64_t Mask, int64_t BitIndex)
		{
			return (Mask | (1 << BitIndex));
		}

		static int32_t Disable(int32_t Mask, int32_t BitIndex)
		{
			return Disable((int64_t)Mask, (int64_t)BitIndex);
		}
		static long Disable(int64_t Mask, int64_t BitIndex)
		{
			return (Mask ^ (1 << BitIndex));
		}

		static int32_t Toggle(int32_t Mask, int32_t BitIndex)
		{
			return Toggle((int64_t)Mask, (int64_t)BitIndex);
		}
		static long Toggle(int64_t Mask, int64_t BitIndex)
		{
			return (Mask ^ (1 << BitIndex));
		}

		static bool IsEnabled(int32_t Mask, int32_t BitIndex)
		{
			return IsEnabled((int64_t)Mask, (int64_t)BitIndex);
		}
		static bool IsEnabled(int64_t Mask, int64_t BitIndex)
		{
			return (Mask & (1 << BitIndex) != 0);
		}
	};
}

#endif