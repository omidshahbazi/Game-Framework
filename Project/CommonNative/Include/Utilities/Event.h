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
		typedef std::shared_ptr<CallbackSignature> CalbackSignaturePointer;

	public:
		Event<ArgsT...>& operator +=(CallbackSignature Callback)
		{
			m_Callbacks.push_back(std::make_shared<CallbackSignature>(std::move(Callback)));

			return *this;
		}

		Event<ArgsT...>& operator -=(CallbackSignature Callback)
		{
			//m_Callbacks.erase(std::remove_if(m_Callbacks.begin(), m_Callbacks.end(), [](auto&& ptr) { return !ptr.lock(); }), m_Callbacks.end());

			m_Callbacks.erase(std::find(m_Callbacks.begin(), m_Callbacks.end(), std::make_shared<CallbackSignature>(std::move(Callback))));

			return *this;
		}

		void operator()(ArgsT... Args)
		{
			for (auto& callback : m_Callbacks)
				(*callback)(std::forward<ArgsT>(Args)...);
		}

	private:
		std::vector<CalbackSignaturePointer> m_Callbacks;
	};
}

#endif