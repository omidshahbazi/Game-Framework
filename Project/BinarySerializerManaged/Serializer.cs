// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

					if (valueType.IsArray)
					{
						SerializeArray(value, Buffer);
					}
					else if (valueType.IsPrimitive)
					{
						WritePrimitive(Buffer, value, valueType);
					}
					else if (valueType.IsEnum)
					{
						Buffer.WriteString(value.ToString());
					}
					else if (valueType == typeof(string))
					{
						Buffer.WriteString((string)value);
					}
					else
					{
						SerializeObject(value, Buffer);
					}
				}
			}

			public static void SerializeArray(object Instance, BufferStream Buffer)
			{
				//Buffer.BeginWriteArray()
			}

			private static void WritePrimitive(BufferStream Buffer, object Value, Type Type)
			{
				int size = (int)Type.GetSizeOf();

				byte[] buffer = new byte[size];

				unsafe
				{
					TypedReference valueRef = __makeref(Value);
					IntPtr valuePtr = **(IntPtr**)(&valueRef);
					Marshal.Copy(valuePtr, buffer, 0, size);
				}

				Buffer.WriteBytes(buffer);
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

					object value = null;

					if (valueType.IsArray)
					{
						value = DeserializeArray(valueType, Buffer);
					}
					else if (valueType.IsPrimitive)
					{
						value = ReadPrimitive(Buffer, valueType);
					}
					else if (valueType.IsEnum)
					{
						string name = Buffer.ReadString();
						value = Enum.Parse(Type, name.ToString());
					}
					else if (valueType == typeof(string))
					{
						value = Buffer.ReadString();
					}
					else
					{
						value = DeserializeObject(valueType, Buffer);
					}

					if (member is FieldInfo)
						((FieldInfo)member).SetValue(instance, value);
					else if (member is PropertyInfo)
						((PropertyInfo)member).SetValue(instance, value, null);
				}

				return instance;
			}

			public static object DeserializeArray(Type Type, BufferStream Buffer)
			{
				return null;
			}

			private static object ReadPrimitive(BufferStream Buffer, Type Type)
			{
				uint size = Type.GetSizeOf();

				byte[] buffer = new byte[size];
				Buffer.ReadBytes(buffer, 0, size);

				unsafe
				{
					TypedReference valueRef = __makeref(Value);
					IntPtr valuePtr = **(IntPtr**)(&valueRef);
					Marshal.Copy(valuePtr, buffer, 0, size);
				}

				Buffer.WriteBytes(buffer);
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
