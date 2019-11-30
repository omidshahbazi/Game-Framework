// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef EVENT_H
#define EVENT_H

#include <functional>
#include <vector>

namespace GameFramework::Common::Utilities
{
	template<typename... ArgsT>
	class Event
	{
	private:
		typedef std::function<void(ArgsT...)> CallbackSignature;

	public:
		Event<ArgsT...>& operator +=(CallbackSignature Callback)
		{
			m_Callbacks.push_back(Callback);

			return *this;
		}

		Event<ArgsT...>& operator -=(CallbackSignature Callback)
		{
			m_Callbacks.erase(std::find(m_Callbacks.begin(), m_Callbacks.end(), Callback));

			return *this;
		}

		void operator()(ArgsT... Args)
		{
			for (CallbackSignature& callback : m_Callbacks)
				callback(std::forward<ArgsT>(Args)...);
		}

	private:
		std::vector<CallbackSignature> m_Callbacks;
	};
}

#endif