// Copyright 2019. All Rights Reserved.
#pragma once
#ifndef CALLBACK_UTIILITIES_H
#define CALLBACK_UTIILITIES_H

#include "Common.h"
#include <Utilities\Event.h>

using namespace GameFramework::Common::Utilities;

namespace GameFramework::Networking
{
	static class NETWORKING_API CallbackUtilities
	{
	public:
		template<typename ...ArgsT>
		static void InvokeCallback(Event<ArgsT...>& Callback, ArgsT... Args)
		{
			try
			{
				Callback(std::forward<ArgsT>(Args)...);
			}
			catch (std::exception e)
			{
				throw e;
			}
		}
	};
}

#endif