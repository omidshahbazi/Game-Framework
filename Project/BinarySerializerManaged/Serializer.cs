// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using Zorvan.Framework.Common.Diagnostics;
using System.IO;

namespace Zorvan.Framework.BinarySerializer
{
	////|value|
	////|array|count|element-1|element-n|
	//public class Serializer
	//{
	//	public MemoryStream Data
	//	{
	//		get;
	//		private set;
	//	}

	//	public Serializer(MemoryStream Data)
	//	{
	//		this.Data = Data;
	//	}

	//	public void WriteBytes(byte[] Value, int Index, int Length)
	//	{
	//		Development.Assert(Data.CanWrite, "Data Can not Write");

	//		Data.Write(Value, Index, Length);
	//	}

	//	public void WriteBytes(byte[] Value)
	//	{
	//		WriteBytes(Value, 0, Value.Length);
	//	}

	//	public byte[] ReadBytes(int Length)
	//	{
	//		Development.Assert(Data.CanRead, "Data Can not Read");

	//		byte[] result = new byte[Length];

	//		for (int i = 0; i < Length; ++i)
	//		{
	//			int value = Data.ReadByte();

	//			Development.Assert(value != -1, "End Of Stream");

	//			result[i] = (byte)value;
	//		}

	//		return result;
	//	}

	//	public void WriteBool(bool Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public void WriteInt32(int Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public void WriteInt64(long Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public void WriteUInt32(uint Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public void WriteFloat32(float Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public void WriteFloat64(double Value)
	//	{
	//		WriteBytes(BitConverter.GetBytes(Value));
	//	}

	//	public bool ReadBool()
	//	{
	//		return BitConverter.ToBoolean(ReadBytes(sizeof(bool)), 0);
	//	}

	//	public int ReadInt32()
	//	{
	//		return BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);
	//	}

	//	public long ReadInt64()
	//	{
	//		return BitConverter.ToInt64(ReadBytes(sizeof(long)), 0);
	//	}

	//	public uint ReadUInt32()
	//	{
	//		return BitConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);
	//	}

	//	public float ReadFloat32()
	//	{
	//		return BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);
	//	}

	//	public double ReadFloat64()
	//	{
	//		return BitConverter.ToDouble(ReadBytes(sizeof(double)), 0);
	//	}

	//	public void BeginWriteArray(int Length)
	//	{
	//		WriteInt32(Length);
	//	}

	//	public void EndWriteArray()
	//	{
	//	}

	//	public void BeginWriteArrayElement()
	//	{
	//	}

	//	public void EndWriteArrayElement()
	//	{
	//	}

	//	public int BeginReadArray()
	//	{
	//		return ReadInt32();
	//	}

	//	public void ReadArrayElement()
	//	{
	//	}
	//}


	//|value|
	//|array|count|element-1|element-n|
	public class Serializer
	{
		public MemoryStream Data
		{
			get;
			private set;
		}

		private byte[] buffer = null;
		private int index = 0;

		public Serializer(MemoryStream Data)
		{
			this.Data = Data;

			if (Data.Length != 0)
				buffer = Data.ToArray();
		}

		public void WriteBytes(byte[] Value, int Index, int Length)
		{
			Development.Assert(Data.CanWrite, "Data Can not Write");

			Data.Write(Value, Index, Length);
		}

		public void WriteBytes(byte[] Value)
		{
			WriteBytes(Value, 0, Value.Length);
		}

		public byte[] ReadBytes(int Length)
		{
			Development.Assert(Data.CanRead, "Data Can not Read");

			byte[] result = new byte[Length];

			for (int i = 0; i < Length; ++i)
			{
				int value = Data.ReadByte();

				Development.Assert(value != -1, "End Of Stream");

				result[i] = (byte)value;
			}

			return result;
		}

		public void WriteBool(bool Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public void WriteInt32(int Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public void WriteInt64(long Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public void WriteUInt32(uint Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public void WriteFloat32(float Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public void WriteFloat64(double Value)
		{
			WriteBytes(BitConverter.GetBytes(Value));
		}

		public bool ReadBool()
		{
			bool value = BitConverter.ToBoolean(buffer, index);
			index += sizeof(bool);
			return value;
		}

		public int ReadInt32()
		{
			int value = BitConverter.ToInt32(buffer, index);
			index += sizeof(int);
			return value;
		}

		public long ReadInt64()
		{
			long value = BitConverter.ToInt64(buffer, index);
			index += sizeof(long);
			return value;
		}

		public uint ReadUInt32()
		{
			uint value = BitConverter.ToUInt32(buffer, index);
			index += sizeof(uint);
			return value;
		}

		public float ReadFloat32()
		{
			float value = BitConverter.ToSingle(buffer, index);
			index += sizeof(float);
			return value;
		}

		public double ReadFloat64()
		{
			double value = BitConverter.ToDouble(buffer, index);
			index += sizeof(double);
			return value;
		}

		public void BeginWriteArray(int Length)
		{
			WriteInt32(Length);
		}

		public void EndWriteArray()
		{
		}

		public void BeginWriteArrayElement()
		{
		}

		public void EndWriteArrayElement()
		{
		}

		public int BeginReadArray()
		{
			return ReadInt32();
		}

		public void ReadArrayElement()
		{
		}
	}
}
