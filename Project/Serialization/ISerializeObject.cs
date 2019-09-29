// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;

namespace Zorvan.Framework.Serialization
{
	public interface ISerializeObject : ISerializeData
	{
		object this[string Name]
		{
			get;
			set;
		}

		bool Contains(string Name);

		ISerializeArray AddArray(string Name);
		ISerializeObject AddObject(string Name);

		void Set(string Name, object Value);
		void Set(string Name, bool Value);
		void Set(string Name, int Value);
		void Set(string Name, uint Value);
		void Set(string Name, float Value);
		void Set(string Name, double Value);
		void Set(string Name, string Value);
		void Set(string Name, Enum Value);

		T Get<T>(string Name, T DefaultValue = default(T));

		void Remove(string Name);

		IEnumerator<KeyValuePair<string, object>> GetEnumerator();

		ISerializeObject Clone();
	}
}
