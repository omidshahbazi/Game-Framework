// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef BUFFER_STREAM_H
#define BUFFER_STREAM_H

#include <cstddef>
#include <inttypes.h>
#include <string>

using namespace std;

namespace GameFramework::BinarySerializer
{
	//|value|
	//|array|count|element-1|element-n|
	class BufferStream
	{
	public:
		BufferStream(byte* const Buffer, uint32_t Length);

		BufferStream(byte* const Buffer, uint32_t Index, uint32_t Length);

		void Reset(void);

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

		void WriteFloat64(double Value);

		void WriteString(wstring Value);

		void WriteBytes(byte* const Data, uint32_t Index, uint32_t Length);

		void BeginWriteArray(uint32_t Length);

		void EndWriteArray(void);

		void BeginWriteArrayElement(void);

		void EndWriteArrayElement(void);

		uint32_t BeginReadArray(void);

		void ReadArrayElement(void);

		void Print(uint32_t BytesPerLine = 8) const;

		__forceinline const byte* GetBuffer(void) const
		{
			return m_Buffer;
		}

		__forceinline uint32_t GetSize(void) const
		{
			return m_Size;
		}

	private:
		byte* m_Buffer;
		uint32_t m_ReadIndex;
		uint32_t m_Size;
	};
}

#endif