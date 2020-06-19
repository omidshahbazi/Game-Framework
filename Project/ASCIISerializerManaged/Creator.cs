// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer.JSONSerializer;
using GameFramework.Common.Extensions;
using GameFramework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameFramework.ASCIISerializer
{
	public static class Creator
	{
		private const string TYPE_FIELD_NAME = "__TYPE";

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
				if (Data == null)
					throw new NullReferenceException("Data cannot be null");

				if (Type.IsArray != (Data is ISerializeArray))
					throw new ArgumentException("Type [" + Type + "] and [" + Data.GetType() + "] are not same");

				Type elementType = (Type.IsArray ? Type.GetElementType() : Type);

				if (Data is ISerializeObject)
				{
					ISerializeObject obj = (ISerializeObject)Data;

					if (obj.Contains(TYPE_FIELD_NAME))
						elementType = Type.GetType(obj.Get<string>(TYPE_FIELD_NAME));

					return Bind(obj, elementType);
				}

				return Bind((ISerializeArray)Data, Type);
			}

			private static object Bind(ISerializeObject Object, Type Type)
			{
				object obj = Activator.CreateInstance(Type);

				MemberInfo[] members = Type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);

				for (int i = 0; i < members.Length; ++i)
				{
					MemberInfo member = members[i];

					if (!Object.Contains(member.Name))
						continue;

					Type memberType = null;
					if (member is FieldInfo)
						memberType = ((FieldInfo)member).FieldType;
					else if (member is PropertyInfo)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;

						if (!propertyInfo.CanWrite)
							continue;

						memberType = propertyInfo.PropertyType;
					}

					object value = Cast(Object.Get<object>(member.Name), memberType);

					if (member is FieldInfo)
						((FieldInfo)member).SetValue(obj, value);
					else if (member is PropertyInfo)
						((PropertyInfo)member).SetValue(obj, value, null);
				}

				return obj;
			}

			private static object Bind(ISerializeArray Array, Type Type)
			{
				Array arr = null;

				Type elementType = Type.GetElementType();

				if (Type.Name.Contains(",")) // Multidimension
				{
					List<object> dimensions = new List<object>();
					ISerializeData data = Array;
					while (true)
					{
						ISerializeArray arrTemp = (ISerializeArray)data;

						dimensions.Add((int)arrTemp.Count);

						if (arrTemp.Count == 0)
							break;

						data = arrTemp.Get<ISerializeArray>(0);
						if (data == null)
							break;

						if (!(data is ISerializeArray))
							break;
					}

					arr = (Array)Activator.CreateInstance(Type, dimensions.ToArray());

					int[] indices = new int[dimensions.Count];

					Bind(Array, elementType, arr, indices, 0);
				}
				else // Jagged
				{
					arr = (Array)Activator.CreateInstance(Type, (int)Array.Count);

					for (uint i = 0; i < Array.Count; ++i)
						arr.SetValue(Cast(Array.Get<object>(i), elementType), i);
				}

				return arr;
			}

			private static void Bind(ISerializeArray Array, Type Type, Array Object, int[] Indices, int DimensionIndex)
			{
				for (uint i = 0; i < Array.Count; ++i)
				{
					if (DimensionIndex < Indices.Length - 1)
					{
						if (DimensionIndex < Indices.Length - 1)
							Indices.Set(0, (uint)DimensionIndex + 1, (uint)(Indices.Length - (DimensionIndex + 1)));

						Bind(Array.Get<ISerializeArray>(i), Type, Object, Indices, DimensionIndex + 1);
					}
					else
						Object.SetValue(Cast(Array.Get<object>(i), Type), Indices);

					++Indices[DimensionIndex];
				}
			}

			private static object Cast(object Value, Type Type)
			{
				if (Value is ISerializeData)
					return Bind((ISerializeData)Value, Type);

				if (Type.IsEnum)
				{
					string itemName = Value.ToString();

					if (Enum.IsDefined(Type, itemName))
						return Enum.Parse(Type, itemName);

					return 0;
				}

				try
				{
					return Convert.ChangeType(Value, Type);
				}
				catch
				{ }

				try
				{
					return Activator.CreateInstance(Type, Value);
				}
				catch
				{ }

				throw new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.", Value.GetType(), Type));
			}
		}

		private static class ObjectSerializer
		{
			public static ISerializeData Serialize(object Instance)
			{
				if (Instance == null)
					throw new NullReferenceException("Instance cannot be null");

				Type type = Instance.GetType();

				if (type.IsArray)
					return SerializeArray(Instance);

				return SerializeObject(Instance);
			}

			private static ISerializeObject SerializeObject(object Instance)
			{
				ISerializeObject obj = Creator.Create<ISerializeObject>();

				Type type = Instance.GetType();

				MemberInfo[] members = type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);

				for (int i = 0; i < members.Length; ++i)
				{
					MemberInfo member = members[i];

					string name = member.Name;
					Type memberType = null;
					object value = null;

					if (member is FieldInfo)
					{
						FieldInfo filedInfo = (FieldInfo)member;
						memberType = filedInfo.FieldType;
						value = filedInfo.GetValue(Instance);
					}
					else if (member is PropertyInfo)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;

						if (!propertyInfo.CanRead)
							continue;

						memberType = propertyInfo.PropertyType;
						value = propertyInfo.GetValue(Instance, null);
					}

					if (value == null)
						obj.Set(name, (object)null);
					else
					{
						Type valueType = value.GetType();

						bool shouldAddValueTypeInfo = (valueType != memberType);

						if (valueType.IsArray)
							obj.Set(name, SerializeArray(value));
						else if (valueType.IsPrimitive || valueType == typeof(string))
							obj.Set(name, value);
						else if (valueType.IsEnum)
							obj.Set(name, value.ToString());
						else
						{
							ISerializeObject subObj = SerializeObject(value);

							if (shouldAddValueTypeInfo)
								subObj.Set(TYPE_FIELD_NAME, valueType.GetMinimalTypeName());

							obj.Set(name, subObj);
						}
					}
				}

				return obj;
			}

			private static ISerializeArray SerializeArray(object Instance)
			{
				object[] array = ((Array)Instance).ToJaggedArray();

				ISerializeArray arr = Creator.Create<ISerializeArray>();

				for (int i = 0; i < array.Length; ++i)
				{
					object value = array[i];

					if (value == null)
						arr.Add((object)null);
					else
					{
						Type valueType = value.GetType();

						if (valueType.IsArray)
							arr.Add(SerializeArray(value));
						else if (valueType.IsPrimitive || valueType == typeof(string))
							arr.Add(value);
						else if (valueType.IsEnum)
							arr.Add(value.ToString());
						else
							arr.Add(SerializeObject(value));
					}
				}

				return arr;
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

			return Bind<T>(data);
		}

		public static T Bind<T>(ISerializeData Data)
		{
			return (T)Bind(typeof(T), Data);
		}

		public static object Bind(Type Type, ISerializeData Data)
		{
			if (Data.GetType() == Type)
				return Data;

			return ObjectBinder.Bind(Data, Type);
		}

		public static T Serialize<T>(object Instance) where T : ISerializeData
		{
			return (T)ObjectSerializer.Serialize(Instance);
		}

		public static void Override<T>(T Data, T On) where T : ISerializeData
		{
			Overrider.Override(Data, On);
		}
	}
}