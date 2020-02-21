// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef EVENT_H
#define EVENT_H

#include <functional>
#include <vector>
#include <algorithm>

namespace GameFramework::Common::Utilities
{
	template<typename... ArgsT>
	using EventHandler = std::function<void(ArgsT...)>;

	template<typename... ArgsT>
	class Event;

	template<typename... ArgsT>
	class EventHandlerInfo
	{
		friend class Event<ArgsT...>;

	public:
		EventHandlerInfo(EventHandler<ArgsT...> Handler) :
			m_ID(++m_LastID),
			m_Handler(Handler)
		{
		}

		void Remove(Event<ArgsT...>& Event) const;

		void operator()(ArgsT... Args)
		{
			m_Handler(std::forward<ArgsT>(Args)...);
		}

	private:
		int m_ID;
		EventHandler<ArgsT...> m_Handler;
		static int m_LastID;
	};

	template<typename... ArgsT>
	class Event
	{
	public:
		EventHandlerInfo<ArgsT...> Add(EventHandler<ArgsT...> Handler)
		{
			EventHandlerInfo handler(Handler);
			m_Handlers.push_back({ handler.m_ID, handler });
			return handler;
		}

		void Add(EventHandlerInfo<ArgsT...>& Info)
		{
			m_Handlers.push_back({ Info.m_ID, Info });
		}

		void Remove(const EventHandlerInfo<ArgsT...>& Info)
		{
			auto it = std::find_if(m_Handlers.begin(), m_Handlers.end(), [Info](auto& pair)
				{
					return (Info.m_ID == pair.second.m_ID);
				});

			m_Handlers.erase(it);
		}

		void operator()(ArgsT... Args)
		{
			for (auto& handler : m_Handlers)
				handler.second(std::forward<ArgsT>(Args)...);
		}

	private:
		std::vector<std::pair<int, EventHandlerInfo<ArgsT...>>> m_Handlers;
	};

	typedef Event<> SimpleEvent;

	template<typename... ArgsT>
	void EventHandlerInfo<ArgsT...>::Remove(Event<ArgsT...>& Event) const
	{
		Event.Remove(*this);
	}

	template<typename... ArgsT>
	int EventHandlerInfo<ArgsT...>::m_LastID = 0;
}

#endif