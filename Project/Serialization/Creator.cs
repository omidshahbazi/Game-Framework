// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using Zorvan.Framework.Common.Reflection;
using Zorvan.Framework.Serialization.JSONSerializer;

namespace Zorvan.Framework.Serialization
{
	public static class Creator
	{
		private static System.Collections.Generic.Dictionary<System.Type, Serializer> serializers = new System.Collections.Generic.Dictionary<System.Type, Serializer>();

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

		public static Serializer GetSerializer(System.Type Type)
		{
			return serializers[(Type.IsArray() ? Type.GetArrayElementType() : (Type.IsList() ? Type.GetListElementType() : Type))];
		}

		public static void AddSerializer(Serializer Serializer)
		{
			serializers[Serializer.Type] = Serializer;
		}
	}
}