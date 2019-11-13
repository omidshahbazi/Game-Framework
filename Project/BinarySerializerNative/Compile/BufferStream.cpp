// Copyright 2019. All Rights Reserved.
#include "..\Include\BufferStream.h"
#include "..\Include\Utilities.h"
#include <memory>
#include <iostream>

namespace GameFramework::BinarySerializer
{
	template<typename T>
	BytesOf<T> ReadBytesOf(byte* const Buffer, uint32_t& Index)
	{
		BytesOf<T> value;

		memcpy(Buffer + Index, value.Bytes, sizeof(T));

		Index += sizeof(T);

		return value;
	}

	template<typename T>
	void WriteBytesOf(T Value, byte* Buffer, uint32_t& Index)
	{
		BytesOf<T> value;
		value.Value = Value;

		memcpy(value.Bytes, Buffer + Index, sizeof(T));

		Index += sizeof(T);

		return value;
	}

	BufferStream::BufferStream(byte* const Buffer, uint32_t Length) : BufferStream(Buffer, 0, Length)
	{
	}

	BufferStream::BufferStream(byte* const Buffer, uint32_t Index, uint32_t Length) :
		m_ReadIndex(0),
		m_Size(0)
	{
		m_Size = Length;
		m_Buffer = reinterpret_cast<byte*>(malloc(m_Size));

		memcpy(m_Buffer, reinterpret_cast<void*>(Buffer + Index), m_Size);
	}

	void BufferStream::Reset(void)
	{
		m_ReadIndex = 0;
		m_Size = 0;
	}

	bool BufferStream::ReadBool(void)
	{
		BytesOf<bool> value = ReadBytesOf<bool>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	int32_t BufferStream::ReadInt32(void)
	{
		BytesOf<int32_t> value = ReadBytesOf<int32_t>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	int64_t BufferStream::ReadInt64(void)
	{
		BytesOf<int64_t> value = ReadBytesOf<int64_t>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	uint32_t BufferStream::ReadUInt32(void)
	{
		BytesOf<uint32_t> value = ReadBytesOf<uint32_t>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	float BufferStream::ReadFloat32(void)
	{
		BytesOf<float> value = ReadBytesOf<float>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	double BufferStream::ReadFloat64(void)
	{
		BytesOf<double> value = ReadBytesOf<double>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	wstring BufferStream::ReadString(void)
	{
		int32_t bufferLen = ReadInt32();

		wstring str(reinterpret_cast<wstring::value_type*>(m_Buffer + m_ReadIndex), bufferLen);

		m_ReadIndex += sizeof(sizeof(wstring::value_type)) * bufferLen;

		return str;
	}

	byte BufferStream::ReadByte(void)
	{
		BytesOf<byte> value = ReadBytesOf<byte>(m_Buffer, m_ReadIndex);

		return value.Value;
	}

	void BufferStream::ReadBytes(byte* Data, uint32_t Index, uint32_t Length)
	{
		memcpy(m_Buffer + m_ReadIndex, Data + Index, Length);
	}

	void BufferStream::WriteBool(bool Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteInt32(int32_t Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteInt64(int64_t Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteUInt32(uint32_t Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteFloat32(float Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteFloat64(double Value)
	{
		WriteBytesOf(Value, m_Buffer, m_Size);
	}

	void BufferStream::WriteString(wstring Value)
	{
		uint32_t size = Value.length() * sizeof(wstring::value_type);
		WriteUInt32(size);
		WriteBytes(reinterpret_cast<byte*>(const_cast<wstring::value_type*>(Value.c_str())), 0, size);
	}

	void BufferStream::WriteBytes(byte* const Data, uint32_t Index, uint32_t Length)
	{
		stream.Write(Data, (int)Index, (int)Length);
		Size += (uint)Data.Length;
	}

	void BufferStream::BeginWriteArray(uint32_t Length)
	{
		WriteUInt32(Length);
	}

	void BufferStream::EndWriteArray(void)
	{
	}

	void BufferStream::BeginWriteArrayElement(void)
	{
	}

	void BufferStream::EndWriteArrayElement(void)
	{
	}

	uint32_t BufferStream::BeginReadArray(void)
	{
		return ReadUInt32();
	}

	void BufferStream::ReadArrayElement(void)
	{
	}

	void BufferStream::Print(uint32_t BytesPerLine = 8) const
	{
		cout << "Size: ";
		cout << m_Size;
		cout << endl;

		int32_t rowCount = (int32_t)ceil(m_Size / (float)BytesPerLine);

		for (int32_t i = 0; i < rowCount; ++i)
		{
			for (int32_t j = 0; j < BytesPerLine; ++j)
			{
				int32_t index = (i * BytesPerLine) + j;

				if (index < m_Size)
					cout << hex << (int32_t)m_Buffer[index];
				else
					cout << ' ';

				cout << ' ';
			}

			cout << '\t';

			for (int32_t j = 0; j < BytesPerLine; ++j)
			{
				int32_t index = (i * BytesPerLine) + j;

				if (index >= m_Size)
					break;

				byte b = m_Buffer[index];

				if (b == (byte)0)
					b = (byte)'.';

				cout << (char)b;
			}

			cout << endl;
		}

		cout << endl;
	}
}