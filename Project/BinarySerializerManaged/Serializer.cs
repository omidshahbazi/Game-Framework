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
		private const byte OBJECT_VALUE_TYPE = 1;
		private const byte ARRAY_VALUE_TYPE = 2;
		private const byte COMPLEX_VALUE_NOT_NULL_STATUS = 0;
		private const byte COMPLEX_VALUE_NULL_STATUS = 1;

		private class TypeMap : Dictionary<uint, Type>
		{ }

		private static TypeMap types;

		static Serializer()
		{
			types = new TypeMap();
		}

		public static void RegisterType<T>()
		{
			RegisterType(typeof(T));
		}

		public static bool RegisterType(Type Type)
		{
			if (Type == null)
				return false;

			uint hash = MakeHash(Type);

			types[hash] = Type;

			return true;
		}

		public static BufferStream Serialize(object Instance)
		{
			BufferStream buffer = new BufferStream(new MemoryStream());

			if (!Serialize(Instance, buffer))
				return null;

			return buffer;
		}

		public static bool Serialize(object Instance, BufferStream Buffer)
		{
			return SerializeObject(Instance, Buffer);
		}

		private static bool SerializeObject(object Instance, BufferStream Buffer)
		{
			if (Buffer == null)
				return false;

			Buffer.WriteBytes(OBJECT_VALUE_TYPE);
			Buffer.WriteBytes(Instance == null ? COMPLEX_VALUE_NULL_STATUS : COMPLEX_VALUE_NOT_NULL_STATUS);

			if (Instance == null)
				return true;

			Type type = Instance.GetType();

			uint hash = MakeHash(type);
			if (!types.ContainsKey(hash))
				return false;

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
					if (!SerializeArray(value, Buffer))
						return false;
				}
				else if (valueType.IsPrimitive)
				{
					WriteBytes(Buffer, value, valueType);
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

			return true;
		}

		private static bool SerializeArray(object Instance, BufferStream Buffer)
		{
			return true;
		}

		public static uint MakeHash<T>()
		{
			return MakeHash(typeof(T));
		}

		public static uint MakeHash(Type Type)
		{
			if (Type == null)
				return 0;

			return CRC32.CalculateHash(Encoding.ASCII.GetBytes(Type.FullName));
		}

		private static void WriteBytes(BufferStream Buffer, object Value, Type Type)
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
}
