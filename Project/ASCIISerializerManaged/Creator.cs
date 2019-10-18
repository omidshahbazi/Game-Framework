// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer.JSONSerializer;

namespace GameFramework.ASCIISerializer
{
	public static class Creator
	{
		private static readonly JSONParser parser = new JSONParser();

		public static T Create<T>() where T : ISerializeData
		{
			if (typeof(T) == typeof(ISerializeObject))
				return (T)(ISerializeData)new JSONSerializeObject(null);
			else
				return (T)(ISerializeData)new JSONSerializeArray(null);
		}

		public static T Create<T>(string Data) where T : ISerializeData
		{
			return (T)parser.Parse(ref Data);
		}
	}
}