// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BUFFER_STREAM_H
#define BUFFER_STREAM_H

#include "Common.h"
#include <cstddef>
#include <inttypes.h>
#include <string>
#include "..\Include\Utilities.h"

using namespace std;

namespace GameFramework::BinarySerializer
{
	//|value|
	//|array|count|element-1|element-n|
	class BINARY_SERIALIZE_API BufferStream
	{
	public:
		BufferStream(uint32_t Capacity);

		BufferStream(byte* const Buffer, uint32_t Length);

		BufferStream(byte* const Buffer, uint32_t Index, uint32_t Length);

		BufferStream(const BufferStream& Other);

		~BufferStream(void);

		void ResetRead(void);

		void ResetWrite(void);

		bool ReadBool(void);

		int32_t ReadInt32(void);

		int64_t ReadInt64(void);

		uint32_t ReadUInt32(void);

		float ReadFloat32(void);

		double ReadFloat64(void);

		wstring ReadString(void);

		byte ReadByte(void);

		void ReadBytes(byte* Data, uint32_t Index, uint32_t Length);

		void WriteBool(bool Value);

		void WriteInt32(int32_t Value);

		void WriteInt64(int64_t Value);

		void WriteUInt32(uint32_t Value);

		void WriteFloat32(float Value);

		void WriteFloat64(const double& Value);

		void WriteString(const char* Value, uint32_t Length);
		void WriteString(const string& Value);

		void WriteString(const wchar_t* Value, uint32_t Length);
		void WriteString(const wstring& Value);

		void WriteBytes(byte Buffer);
		void WriteBytes(byte* const Data, uint32_t Index, uint32_t Length);

		void BeginWriteArray(uint32_t Length);

		void EndWriteArray(void);

		void BeginWriteArrayElement(void);

		void EndWriteArrayElement(void);

		uint32_t BeginReadArray(void);

		void ReadArrayElement(void);

		BufferStream& operator=(const BufferStream& Other);

		void Print(uint32_t BytesPerLine = 8) const;

		__forceinline byte* const GetBuffer(void) const
		{
			return m_Buffer;
		}

		__forceinline uint32_t GetSize(void) const
		{
			return m_Size;
		}

	private:
		void EnsureCapacity(uint32_t AdditonalCapacity);

		template<typename T>
		BytesOf<T> ReadBytesOf(byte* const Buffer, uint32_t& Index)
		{
			BytesOf<T> value;

			ReadBytes(value.Bytes, 0, sizeof(T));

			return value;
		}

		template<typename T>
		void WriteBytesOf(T Value)
		{
			BytesOf<T> value;
			value.Value = Value;

			WriteBytes(value.Bytes, 0, sizeof(T));
		}

	private:
		byte* m_Buffer;
		uint32_t m_Capacity;
		uint32_t m_ReadIndex;
		uint32_t m_Size;
	};
}

#endif