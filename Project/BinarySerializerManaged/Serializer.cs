// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameFramework.BinarySerializer
{
	public static class Serializer
	{
		private const byte COMPLEX_VALUE_NOT_NULL_STATUS = 0;
		private const byte COMPLEX_VALUE_NULL_STATUS = 1;

		private static class Dumper
		{
			public static void SerializeObject(object Instance, BufferStream Buffer)
			{
				Buffer.WriteBytes(Instance == null ? COMPLEX_VALUE_NULL_STATUS : COMPLEX_VALUE_NOT_NULL_STATUS);

				if (Instance == null)
					return;

				Type type = Instance.GetType();

				MemberInfo[] members = type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);

				for (int i = 0; i < members.Length; ++i)
				{
					MemberInfo member = members[i];

					object value = null;
					Type valueType = null;

					if (member is FieldInfo)
					{
						FieldInfo fieldInfo = (FieldInfo)member;
						value = fieldInfo.GetValue(Instance);
						valueType = fieldInfo.FieldType;
					}
					else if (member is PropertyInfo)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;
						value = propertyInfo.GetValue(Instance, null);
						valueType = propertyInfo.PropertyType;
					}

					WriteValue(Buffer, value, valueType);
				}
			}

			public static void SerializeArray(object Instance, BufferStream Buffer)
			{
				Buffer.WriteBytes(Instance == null ? COMPLEX_VALUE_NULL_STATUS : COMPLEX_VALUE_NOT_NULL_STATUS);

				if (Instance == null)
					return;

				Type elementType = Instance.GetType().GetElementType();

				Array array = (Array)Instance;

				Buffer.BeginWriteArray((uint)array.Length);

				for (int i = 0; i < array.Length; ++i)
				{
					Buffer.BeginWriteArrayElement();

					object value = array.GetValue(i);

					WriteValue(Buffer, value, elementType);

					Buffer.EndWriteArrayElement();
				}

				Buffer.EndWriteArray();
			}

			private static void WriteValue(BufferStream Buffer, object Value, Type Type)
			{
				if (Type.IsArray)
				{
					SerializeArray(Value, Buffer);
				}
				else if (Type.IsPrimitive)
				{
					Buffer.WriteBytes(BitwiseHelper.GetBytes(Value, Type));
				}
				else if (Type.IsEnum)
				{
					Buffer.WriteString(Value.ToString());
				}
				else if (Type == typeof(string))
				{
					Buffer.WriteString((string)Value);
				}
				else
				{
					SerializeObject(Value, Buffer);
				}
			}
		}

		private static class Binder
		{
			public static object DeserializeObject(Type Type, BufferStream Buffer)
			{
				byte valueStatus = Buffer.ReadByte();
				if (valueStatus == COMPLEX_VALUE_NULL_STATUS)
					return null;

				object instance = Activator.CreateInstance(Type);

				MemberInfo[] members = Type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);

				for (int i = 0; i < members.Length; ++i)
				{
					MemberInfo member = members[i];

					Type valueType = null;

					if (member is FieldInfo)
					{
						FieldInfo fieldInfo = (FieldInfo)member;
						valueType = fieldInfo.FieldType;
					}
					else if (member is PropertyInfo)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;
						valueType = propertyInfo.PropertyType;
					}

					object value = ReadValue(Buffer, valueType);

					if (member is FieldInfo)
						((FieldInfo)member).SetValue(instance, value);
					else if (member is PropertyInfo)
						((PropertyInfo)member).SetValue(instance, value, null);
				}

				return instance;
			}

			public static object DeserializeArray(Type Type, BufferStream Buffer)
			{
				byte valueStatus = Buffer.ReadByte();
				if (valueStatus == COMPLEX_VALUE_NULL_STATUS)
					return null;

				Type elementType = Type.GetElementType();

				uint count = Buffer.BeginReadArray();

				Array arr = (Array)Activator.CreateInstance(Type, (int)count);

				for (uint i = 0; i < arr.Length; ++i)
					arr.SetValue(ReadValue(Buffer, elementType), i);

				return arr;
			}

			private static object ReadValue(BufferStream Buffer, Type Type)
			{
				object value = null;

				if (Type.IsArray)
				{
					value = DeserializeArray(Type, Buffer);
				}
				else if (Type.IsPrimitive)
				{
					value = ReadPrimitive(Buffer, Type);
				}
				else if (Type.IsEnum)
				{
					string name = Buffer.ReadString();
					value = Enum.Parse(Type, name.ToString());
				}
				else if (Type == typeof(string))
				{
					value = Buffer.ReadString();
				}
				else
				{
					value = DeserializeObject(Type, Buffer);
				}

				return value;
			}

			private static object ReadPrimitive(BufferStream Buffer, Type Type)
			{
				uint size = Type.GetSizeOf();
				byte[] buffer = new byte[size];
				Buffer.ReadBytes(buffer, 0, size);

				return BitwiseHelper.GetObject(Type, buffer);
			}
		}

		private class TypeMap : Dictionary<uint, Type>
		{ }

		public static BufferStream Serialize(object Instance)
		{
			BufferStream buffer = new BufferStream(new MemoryStream());

			Serialize(Instance, buffer);

			return buffer;
		}

		public static void Serialize(object Instance, BufferStream Buffer)
		{
			if (Instance == null)
				throw new NullReferenceException("Instance cannot be null");

			if (Buffer == null)
				throw new NullReferenceException("Buffer cannot be null");

			Type type = Instance.GetType();

			if (type.IsArray)
				Dumper.SerializeArray(Instance, Buffer);

			Dumper.SerializeObject(Instance, Buffer);
		}

		public static T Deserialize<T>(BufferStream Buffer)
		{
			return (T)Deserialize(typeof(T), Buffer);
		}

		public static object Deserialize(Type Type, BufferStream Buffer)
		{
			Type elementType = (Type.IsArray ? Type.GetElementType() : Type);

			if (Type.IsArray)
				return Binder.DeserializeArray(elementType, Buffer);

			return Binder.DeserializeObject(Type, Buffer);
		}
	}
}
