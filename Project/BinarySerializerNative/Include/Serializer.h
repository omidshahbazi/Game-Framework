// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef SERIALIZER_H
#define SERIALIZER_H

#include <streambuf>
#include <vector>

namespace Zorvan::Framework::BinarySerializer
{
	class Serializer
	{
	private:
		template<typename T>
		union BytesOf
		{
		public:
			T Value;
			unsigned char Bytes[sizeof(T)];
		};

	public:
		Serializer(char *Buffer, unsigned int Size);

		void WriteBool(bool Value);
		void WriteInt32(int Value);
		void WriteInt64(long Value);
		void WriteUInt32(unsigned int Value);
		void WriteFloat32(float Value);
		void WriteFloat64(double Value);

		bool ReadBool(void);
		int ReadInt32(void);
		long ReadInt64(void);
		unsigned int ReadUInt32(void);
		float ReadFloat32(void);
		double ReadFloat64(void);

		void BeginArray(void);
		void EndArray(void);

		void BeginArrayElement(void);
		void EndArrayElement(void);

		unsigned int ReadArray(void);
		const char *const ReadArrayElement(void);

		inline void Reset(void)
		{
			m_Index = 0;
			m_ArrayStack.clear();
		}

		inline unsigned int Size(void)
		{
			return m_Index;
		}

	private:
		template<typename T>
		void WriteInBuffer(BytesOf<T> &Value)
		{
			for (int i = 0; i < sizeof(T); ++i)
				m_Buffer[m_Index++] = Value.Bytes[i];
		}

		template<typename T>
		BytesOf<T> ReadFromBuffer(void)
		{
			BytesOf<T> value;

			for (int i = 0; i < sizeof(T); ++i)
				value.Bytes[i] = m_Buffer[m_Index++];

			return value;
		}

	public:
		char *m_Buffer;
		unsigned int m_Size;
		unsigned int m_Index;
		std::vector<int> m_ArrayStack;
	};
}

#endif