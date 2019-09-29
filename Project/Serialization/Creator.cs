// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using Zorvan.Framework.Serialization.JSONSerializer;

namespace Zorvan.Framework.Serialization
{
	public static class Creator
	{
		public static T Create<T>() where T : ISerializeData
		{
			if (typeof(T) == typeof(ISerializeObject))
				return (T)(ISerializeData)new JSONSerializeObject(null);
			else
				return (T)(ISerializeData)new JSONSerializeArray(null);
		}

		public static T Create<T>(string Data) where T : ISerializeData
		{
			return JSONSerializeObject.Deserialize<T>(Data);
		}
	}
}