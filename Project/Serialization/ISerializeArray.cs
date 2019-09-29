﻿// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;

namespace Zorvan.Framework.Serialization
{
	public interface ISerializeArray : ISerializeData
	{
		object this[uint Index]
		{
			get;
			set;
		}

		ISerializeArray AddArray();
		ISerializeObject AddObject();

		void Add(object Item);
		void Add(bool Item);
		void Add(int Item);
		void Add(uint Item);
		void Add(float Item);
		void Add(double Item);
		void Add(string Item);
		void Add(Enum Item);

		T Get<T>(uint Index, T DefaultValue = default(T));

		void Clear();

		void AddRange<T>(T[] Range);
		T[] GetRange<T>();
		T[] GetRange<T>(uint Index, uint Count);

		void Remove(uint Index);

		ISerializeArray Clone();
	}
}
