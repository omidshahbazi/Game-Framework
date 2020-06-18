// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using GameFramework.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GameFramework.BinarySerializer
{
	public static class Serializer
	{
		private const byte COMPLEX_VALUE_NOT_NULL_STATUS = 0;
		private const byte COMPLEX_VALUE_NULL_STATUS = 1;
		private const byte ARRAY_MULTIDIMENSION_STATUS = 0;
		private const byte ARRAY_JAGGED_STATUS = 1;

		private static class Dumper
		{
			public static void SerializeObject(object Instance, BufferStream Buffer)
			{
				SerializeObject(Instance, Instance.GetType(), Buffer);
			}

			public static void SerializeArray(object Instance, BufferStream Buffer)
			{
				Buffer.WriteBytes(Instance == null ? COMPLEX_VALUE_NULL_STATUS : COMPLEX_VALUE_NOT_NULL_STATUS);

				if (Instance == null)
					return;

				Array multidimensionArray = (Array)Instance;
				object[] jaggedArray = multidimensionArray.ToJaggedArray();

				bool isMultidimension = Instance.GetType().Name.Contains(",");
				Buffer.WriteBytes(isMultidimension ? ARRAY_MULTIDIMENSION_STATUS : ARRAY_JAGGED_STATUS);

				if (isMultidimension) // Multidimension
				{
					Buffer.WriteInt32(multidimensionArray.Rank);
					for (int i = 0; i < multidimensionArray.Rank; ++i)
						Buffer.WriteInt32(multidimensionArray.GetLength(i));
				}

				Buffer.BeginWriteArray((uint)jaggedArray.Length);

				for (int i = 0; i < jaggedArray.Length; ++i)
				{
					Buffer.BeginWriteArrayElement();

					object value = jaggedArray.GetValue(i);

					WriteValue(Buffer, value, value.GetType());

					Buffer.EndWriteArrayElement();
				}

				Buffer.EndWriteArray();
			}

			private static void SerializeObject(object Instance, Type MemberType, BufferStream Buffer)
			{
				Buffer.WriteBytes(Instance == null ? COMPLEX_VALUE_NULL_STATUS : COMPLEX_VALUE_NOT_NULL_STATUS);

				if (Instance == null)
					return;

				Type type = Instance.GetType();

				bool shouldAddValueTypeInfo = (type != MemberType);

				Buffer.WriteBool(shouldAddValueTypeInfo);
				if (shouldAddValueTypeInfo)
					Buffer.WriteString(type.GetMinimalTypeName());

				MemberInfo[] members = type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);

				Utilities.CheckForDuplicateID(members);

				Buffer.WriteUInt16((ushort)members.Length);

				BufferStream tempBuffer = new BufferStream(new MemoryStream());

				for (int i = 0; i < members.Length; ++i)
				{
					MemberInfo member = members[i];

					uint id = Utilities.GetMemberID(member);

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

						if (!propertyInfo.CanRead)
							continue;

						value = propertyInfo.GetValue(Instance, null);
						valueType = propertyInfo.PropertyType;
					}

					tempBuffer.ResetWrite();
					WriteValue(tempBuffer, value, valueType);

					Buffer.WriteUInt32(id);
					Buffer.WriteUInt16((ushort)tempBuffer.Size);
					Buffer.WriteBytes(tempBuffer.Buffer);
				}
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
					SerializeObject(Value, Type, Buffer);
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

				bool containsValueTypeInfo = Buffer.ReadBool();
				if (containsValueTypeInfo)
					Type = Type.GetType(Buffer.ReadString());

				object instance = Activator.CreateInstance(Type);

				ushort memberCount = Buffer.ReadUInt16();

				MemberInfo[] members = Type.GetMemberVariables(ReflectionExtensions.AllNonStaticFlags);
				uint[] ids = GetIDs(members);

				byte[] skippedBytes = new byte[ushort.MaxValue + 1];
				for (ushort __i = 0; __i < memberCount; ++__i)
				{
					uint id = Buffer.ReadUInt32();
					ushort size = Buffer.ReadUInt16();

					int index = Array.IndexOf(ids, id);
					if (index == -1)
					{
						Buffer.ReadBytes(skippedBytes, 0, size);
						continue;
					}

					MemberInfo member = members[index];

					Type valueType = null;

					if (member is FieldInfo)
					{
						FieldInfo fieldInfo = (FieldInfo)member;
						valueType = fieldInfo.FieldType;
					}
					else if (member is PropertyInfo)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;

						if (!propertyInfo.CanWrite)
							continue;

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
				uint count;
				List<object> dimensions = new List<object>();
				if (!ReadArrayHeader(Buffer, Type, out count, dimensions))
					return null;

				Array arr = null;

				Type elementType = Type.GetElementType();

				if (Type.Name.Contains(",")) // Multidimension
				{
					arr = (Array)Activator.CreateInstance(Type, dimensions.ToArray());

					int[] indices = new int[dimensions.Count];

					DeserializeArray(elementType, Buffer, count, arr, indices, 0);
				}
				else // Jagged
				{
					arr = (Array)Activator.CreateInstance(Type, (int)count);

					for (uint i = 0; i < arr.Length; ++i)
						arr.SetValue(ReadValue(Buffer, elementType), i);
				}

				return arr;
			}

			private static bool ReadArrayHeader(BufferStream Buffer, Type Type, out uint ElementCount, List<object> Dimensions)
			{
				ElementCount = 0;

				byte valueStatus = Buffer.ReadByte();
				if (valueStatus == COMPLEX_VALUE_NULL_STATUS)
					return false;

				if (Buffer.ReadByte() == ARRAY_MULTIDIMENSION_STATUS)
				{
					int dimensionCount = Buffer.ReadInt32();
					for (int i = 0; i < dimensionCount; ++i)
						Dimensions.Add(Buffer.ReadInt32());
				}

				ElementCount = Buffer.BeginReadArray();

				return true;
			}

			private static void DeserializeArray(Type Type, BufferStream Buffer, uint Count, Array Object, int[] Indices, int DimensionIndex)
			{
				for (uint i = 0; i < Count; ++i)
				{
					if (DimensionIndex < Indices.Length - 1)
					{
						if (DimensionIndex < Indices.Length - 1)
							Indices.Set(0, (uint)DimensionIndex + 1, (uint)(Indices.Length - (DimensionIndex + 1)));

						uint count;
						List<object> dimensions = new List<object>();
						if (!ReadArrayHeader(Buffer, Type, out count, dimensions))
						{
							Object.SetValue(null, Indices);

							continue;
						}

						DeserializeArray(Type, Buffer, count, Object, Indices, DimensionIndex + 1);
					}
					else
						Object.SetValue(ReadValue(Buffer, Type), Indices);

					++Indices[DimensionIndex];
				}
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

					if (Enum.IsDefined(Type, name))
						value = Enum.Parse(Type, name);
					else
						value = 0;
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

			private static uint[] GetIDs(MemberInfo[] Members)
			{
				uint[] ids = new uint[Members.Length];

				for (int i = 0; i < Members.Length; ++i)
					ids[i] = Utilities.GetMemberID(Members[i]);

				return ids;
			}
		}

		private static class Utilities
		{
			public static uint GetMemberID(MemberInfo Member)
			{
				KeyAttribute keyAttr = Member.GetAttribute<KeyAttribute>();

				if (keyAttr == null)
					return CRC32.CalculateHash(Encoding.ASCII.GetBytes(Member.Name));

				return keyAttr.ID;
			}

			public static void CheckForDuplicateID(MemberInfo[] Members)
			{
				List<KeyAttribute> attributes = new List<KeyAttribute>();

				for (int i = 0; i < Members.Length; ++i)
				{
					KeyAttribute keyAttr = Members[i].GetAttribute<KeyAttribute>();
					if (keyAttr == null)
						continue;

					attributes.Add(keyAttr);
				}

				for (int i = 0; i < attributes.Count; ++i)
					for (int j = i + 1; j < attributes.Count; ++j)
						if (attributes[i].ID == attributes[j].ID)
							throw new ArgumentException("Key [" + attributes[i].ID + "] is duplicate");
			}
		}

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
