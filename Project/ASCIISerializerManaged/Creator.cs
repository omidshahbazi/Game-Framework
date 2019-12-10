// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer.JSONSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameFramework.ASCIISerializer
{
	public static class Creator
	{
		private static class Overrider
		{
			public static void Override(ISerializeData Data, ISerializeData On)
			{
				if (Data is ISerializeObject)
					Override((ISerializeObject)Data, (ISerializeObject)On);
				else
					Override((ISerializeArray)Data, (ISerializeArray)On);
			}

			public static void Override(ISerializeObject Data, ISerializeObject On)
			{
				var it = Data.GetEnumerator();
				while (it.MoveNext())
				{
					string key = it.Current.Key;
					object value = it.Current.Value;

					object onValue = On.Get<object>(key);

					if (value is ISerializeObject)
					{
						ISerializeObject onObj = null;
						if (onValue != null)
						{
							if (onValue is ISerializeObject)
								onObj = (ISerializeObject)onValue;
							else
							{
								Data.Remove(key);

								it = Data.GetEnumerator();
							}
						}

						if (onObj == null)
							onObj = On.AddObject(key);

						Override((ISerializeData)value, onObj);
					}
					else if (value is ISerializeArray)
					{
						if (onValue != null)
							Data.Remove(key);

						it = Data.GetEnumerator();

						Override((ISerializeData)value, On.AddArray(key));
					}
					else
						On.Set(key, value);
				}
			}

			public static void Override(ISerializeArray Data, ISerializeArray On)
			{
				var it = Data.GetEnumerator();
				while (it.MoveNext())
				{
					object value = it.Current;

					if (value is ISerializeObject)
						Override((ISerializeObject)value, On.AddObject());
					else if (value is ISerializeArray)
						Override((ISerializeArray)value, On.AddArray());
					else
					{
						if (On.Contains(value))
							continue;

						On.Add(value);
					}
				}
			}
		}

		private static class ObjectBinder
		{
			public static T Bind<T>(ISerializeData Data)
			{
				return (T)Bind(Data, typeof(T));
			}

			public static object Bind(ISerializeData Data, Type Type)
			{
				if (Type.IsArray != (Data is ISerializeArray))
					throw new ArgumentException("Type [" + Type + "] and [" + Data.GetType() + "] are not same");

				Type elementType = (Type.IsArray ? Type.GetElementType() : Type);

				if (Data is ISerializeObject)
					return Bind((ISerializeObject)Data, elementType);

				return Bind((ISerializeArray)Data, Type);
			}

			private static object Bind(ISerializeObject Object, Type Type)
			{
				object obj = Activator.CreateInstance(Type);

				List<MemberInfo> members = new List<MemberInfo>();
				members.AddRange(Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				members.AddRange(Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

				for (int i = 0; i < members.Count; ++i)
				{
					MemberInfo member = members[i];

					if (!Object.Contains(member.Name))
						continue;

					Type memberType = null;
					if (member is FieldInfo)
						memberType = ((FieldInfo)member).FieldType;
					else if (member is PropertyInfo)
						memberType = ((PropertyInfo)member).PropertyType;

					object value = Object.Get<object>(member.Name);

					if (value is ISerializeData)
						value = Bind((ISerializeData)value, memberType);
					else
						value = Convert.ChangeType(value, memberType);

					if (member is FieldInfo)
						((FieldInfo)member).SetValue(obj, value);
					else if (member is PropertyInfo)
						((PropertyInfo)member).SetValue(obj, value, null);
				}

				return obj;
			}

			private static object Bind(ISerializeArray Array, Type Type)
			{
				Array obj = (Array)Activator.CreateInstance(Type, (int)Array.Count);

				for (uint i = 0; i < Array.Count; ++i)
				{
					object value = Array.Get<object>(i);

					Type elementType = Type.GetElementType();

					if (value is ISerializeData)
						value = Bind((ISerializeData)value, Type.GetElementType());
					else
						value = Convert.ChangeType(value, elementType);

					obj.SetValue(value, i);
				}

				return obj;
			}
		}

		private static readonly JSONParser parser = new JSONParser();

		public static T Create<T>() where T : ISerializeData
		{
			if (typeof(T) == typeof(ISerializeObject))
				return (T)(ISerializeData)new JSONSerializeObject(null);
			else
				return (T)(ISerializeData)new JSONSerializeArray(null);
		}

		public static T Create<T>(string Data)
		{
			Type type = typeof(T);

			ISerializeData data = parser.Parse(ref Data);

			if (data is T)
				return (T)data;

			return ObjectBinder.Bind<T>(data);
		}

		public static void Override<T>(T Data, T On) where T : ISerializeData
		{
			Overrider.Override(Data, On);
		}
	}
}