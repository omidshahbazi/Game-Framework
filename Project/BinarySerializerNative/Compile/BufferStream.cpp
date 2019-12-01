// Copyright 2019. All Rights Reserved.
#include "..\Include\BufferStream.h"
#include <memory>
#include <iostream>

namespace GameFramework::BinarySerializer
{
	BufferStream::BufferStream(uint32_t Capacity) :
		m_Buffer(nullptr),
		m_Capacity(0),
		m_ReadIndex(0),
		m_Size(0)
	{
		EnsureCapacity(Capacity);
	}

	BufferStream::BufferStream(byte* const Buffer, uint32_t Length) : BufferStream(Buffer, 0, Length)
	{
	}

	BufferStream::BufferStream(byte* const Buffer, uint32_t Index, uint32_t Length) :
		m_Buffer(nullptr),
		m_Capacity(0),
		m_ReadIndex(0),
		m_Size(0)
	{
		m_Size = Length;
		EnsureCapacity(Length);

		memcpy(m_Buffer, Buffer + Index, m_Size);
	}

	BufferStream::BufferStream(const BufferStream& Other) :
		m_Buffer(nullptr),
		m_Capacity(0),
		m_ReadIndex(0),
		m_Size(0)
	{
		*this = Other;
	}

	BufferStream::~BufferStream(void)
	{
		if (m_Buffer != nullptr)
			free(m_Buffer);
	}

	void BufferStream::ResetRead(void)
	{
		m_ReadIndex = 0;
	}

	void BufferStream::ResetWrite(void)
	{
		ResetRead();
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
		memcpy(Data + Index, m_Buffer + m_ReadIndex, Length);

		m_ReadIndex += Length;
	}

	void BufferStream::WriteBool(bool Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteInt32(int32_t Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteInt64(int64_t Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteUInt32(uint32_t Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteFloat32(float Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteFloat64(const double& Value)
	{
		WriteBytesOf(Value);
	}

	void BufferStream::WriteString(const char* Value, uint32_t Length)
	{
		uint32_t size = Length * sizeof(char);
		WriteUInt32(size);

		for (int i = 0; i < Length; ++i)
		{
			wchar_t ch = Value[i];
			WriteBytesOf(ch);
		}
	}

	void BufferStream::WriteString(const string& Value)
	{
		WriteString(Value.c_str(), Value.length());
	}

	void BufferStream::WriteString(const wchar_t* Value, uint32_t Length)
	{
		uint32_t size = Length * sizeof(wchar_t);
		WriteUInt32(size);
		WriteBytes(reinterpret_cast<byte*>(const_cast<wchar_t*>(Value)), 0, size);
	}

	void BufferStream::WriteString(const wstring& Value)
	{
		WriteString(Value.c_str(), Value.length());
	}

	void BufferStream::WriteBytes(byte Buffer)
	{
		WriteBytes(&Buffer, 0, 1);
	}

	void BufferStream::WriteBytes(byte* const Buffer, uint32_t Index, uint32_t Length)
	{
		EnsureCapacity(Length);

		memcpy(m_Buffer + m_Size, Buffer + Index, Length);
		m_Size += Length;
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

	BufferStream& BufferStream::operator=(const BufferStream& Other)
	{
		m_ReadIndex = 0;
		m_Size = 0;

		m_Size = Other.m_Size;
		EnsureCapacity(m_Size);

		memcpy(m_Buffer, Other.m_Buffer, m_Size);

		return *this;
	}

	void BufferStream::Print(uint32_t BytesPerLine) const
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

	void BufferStream::EnsureCapacity(uint32_t AdditonalCapacity)
	{
		if (m_Capacity >= m_Size + AdditonalCapacity)
			return;

		m_Capacity += AdditonalCapacity;

		byte* newBuffer = reinterpret_cast<byte*>(malloc(m_Capacity));

		if (m_Buffer != nullptr)
		{
			memcpy(newBuffer, m_Buffer, m_Size);

			free(m_Buffer);
		}

		m_Buffer = newBuffer;
	}
}