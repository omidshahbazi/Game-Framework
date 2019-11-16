// Copyright 2019. All Rights Reserved.
using System;
using System.IO;
using System.Text;

namespace GameFramework.BinarySerializer
{
	//|value|
	//|array|count|element-1|element-n|
	public class BufferStream
	{
		private MemoryStream stream = null;

		public byte[] Buffer
		{
			get { return stream.ToArray(); }
		}

		public uint Size
		{
			get;
			private set;
		}

		public BufferStream(uint Capacity)
		{
			stream = new MemoryStream((int)Capacity);
		}

		public BufferStream(byte[] Buffer)
		{
			stream = new MemoryStream(Buffer, true);
			Size = (uint)Buffer.Length;
		}

		public BufferStream(byte[] Buffer, uint Length)
		{
			stream = new MemoryStream(Buffer, 0, (int)Length, true);
			Size = Length;
		}

		public BufferStream(byte[] Buffer, uint Index, uint Length)
		{
			stream = new MemoryStream(Buffer, (int)Index, (int)Length, true);
			Size = Length;
		}

		public BufferStream(MemoryStream Stream)
		{
			stream = Stream;
			Size = (uint)Stream.Position;
		}

		public void Reset()
		{
			stream.Seek(0, SeekOrigin.Begin);
			Size = 0;
		}

		public bool ReadBool()
		{
			byte[] data = new byte[sizeof(bool)];
			ReadBytes(data, 0, sizeof(bool));
			return BitConverter.ToBoolean(data, 0);
		}

		public int ReadInt32()
		{
			byte[] data = new byte[sizeof(int)];
			ReadBytes(data, 0, sizeof(int));
			return BitConverter.ToInt32(data, 0);
		}

		public long ReadInt64()
		{
			byte[] data = new byte[sizeof(long)];
			ReadBytes(data, 0, sizeof(long));
			return BitConverter.ToInt64(data, 0);
		}

		public uint ReadUInt32()
		{
			byte[] data = new byte[sizeof(uint)];
			ReadBytes(data, 0, sizeof(uint));
			return BitConverter.ToUInt32(data, 0);
		}

		public float ReadFloat32()
		{
			byte[] data = new byte[sizeof(float)];
			ReadBytes(data, 0, sizeof(float));
			return BitConverter.ToSingle(data, 0);
		}

		public double ReadFloat64()
		{
			byte[] data = new byte[sizeof(double)];
			ReadBytes(data, 0, sizeof(double));
			return BitConverter.ToDouble(data, 0);
		}

		public string ReadString()
		{
			uint bufferLen = ReadUInt32();

			byte[] data = new byte[bufferLen];
			ReadBytes(data, 0, bufferLen);

			return Encoding.UTF8.GetString(data, 0, (int)bufferLen);
		}

		public byte ReadByte()
		{
			return Convert.ToByte(stream.ReadByte());
		}

		public void ReadBytes(byte[] Data, uint Index, uint Length)
		{
			stream.Read(Data, (int)Index, (int)Length);
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

		public void WriteString(string Value)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(Value);
			WriteInt32(buffer.Length);
			WriteBytes(buffer);
		}

		public void WriteBytes(params byte[] Buffer)
		{
			WriteBytes(Buffer, 0, (uint)Buffer.Length);
		}

		public void WriteBytes(byte[] Buffer, uint Index, uint Length)
		{
			stream.Write(Buffer, (int)Index, (int)Length);
			Size += (uint)Buffer.Length;
		}

		public void BeginWriteArray(uint Length)
		{
			WriteUInt32(Length);
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

		public uint BeginReadArray()
		{
			return ReadUInt32();
		}

		public void ReadArrayElement()
		{
		}

		public void Print(int BytesPerLine = 8)
		{
			Console.Write("Size: ");
			Console.Write(Size);
			Console.WriteLine();

			int rowCount = (int)Math.Ceiling(Size / (float)BytesPerLine);

			for (int i = 0; i < rowCount; ++i)
			{
				for (int j = 0; j < BytesPerLine; ++j)
				{
					int index = (i * BytesPerLine) + j;

					string hexValue = "  ";

					if (index < Size)
						hexValue = Buffer[index].ToString("X2");

					Console.Write(hexValue);
					Console.Write(' ');
				}

				Console.Write('\t');

				for (int j = 0; j < BytesPerLine; ++j)
				{
					int index = (i * BytesPerLine) + j;

					if (index >= Size)
						break;

					byte b = Buffer[index];

					if (b == 0)
						b = (byte)'.';

					Console.Write((char)b);
				}

				Console.WriteLine();
			}

			Console.WriteLine();
		}
	}
}
