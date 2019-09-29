// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#include "..\Include\Serializer.h"

namespace Zorvan::Framework::BinarySerializer
{
	Serializer::Serializer(char *Buffer, unsigned int Size) :
		m_Buffer(Buffer),
		m_Size(Size)
	{
		Reset();
	}

	void Serializer::WriteBool(bool Value)
	{
		BytesOf<bool> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	void Serializer::WriteInt32(int Value)
	{
		BytesOf<int> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	void Serializer::WriteInt64(long Value)
	{
		BytesOf<long> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	void Serializer::WriteUInt32(unsigned int Value)
	{
		BytesOf<unsigned int> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	void Serializer::WriteFloat32(float Value)
	{
		BytesOf<float> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	void Serializer::WriteFloat64(double Value)
	{
		BytesOf<double> val;
		val.Value = Value;

		WriteInBuffer(val);
	}

	bool Serializer::ReadBool(void)
	{
		return ReadFromBuffer<bool>().Value;
	}

	int Serializer::ReadInt32(void)
	{
		return ReadFromBuffer<int>().Value;
	}

	long Serializer::ReadInt64(void)
	{
		return ReadFromBuffer<long>().Value;
	}

	unsigned int Serializer::ReadUInt32(void)
	{
		return ReadFromBuffer<unsigned int>().Value;
	}

	float Serializer::ReadFloat32(void)
	{
		return ReadFromBuffer<float>().Value;
	}

	double Serializer::ReadFloat64(void)
	{
		return ReadFromBuffer<double>().Value;
	}

	void Serializer::BeginArray(void)
	{
		m_ArrayStack.push_back(m_Index);

		WriteUInt32(0);
		WriteUInt32(0);
	}

	void Serializer::EndArray(void)
	{
		int startIndex = m_ArrayStack.back();
		m_ArrayStack.pop_back();

		unsigned int size = m_Index - startIndex - (2 * sizeof(unsigned int));
		unsigned int tempIndex = m_Index;
		m_Index = startIndex;

		WriteUInt32(size);

		m_Index = tempIndex;
	}

	void Serializer::BeginArrayElement(void)
	{
	}

	void Serializer::EndArrayElement(void)
	{
		int startIndex = m_ArrayStack.back();
		unsigned int size = m_Index - startIndex - (2 * sizeof(unsigned int));
		unsigned int tempIndex = m_Index;
		m_Index = startIndex + sizeof(unsigned int);

		WriteUInt32(size);

		m_Index = tempIndex;
	}

	unsigned int Serializer::ReadArray(void)
	{
		m_ArrayStack.push_back(m_Index);

		unsigned int elementSize = ReadUInt32();
		unsigned int totalSize = ReadUInt32();

		if (elementSize == 0)
			return 0;

		return totalSize / elementSize;
	}

	const char * const Serializer::ReadArrayElement(void)
	{
		int startIndex = m_ArrayStack.back();
		unsigned int tempIndex = m_Index;
		m_Index = startIndex;

		unsigned int totalSize = ReadInt32();
		unsigned int elementSize = ReadInt32();

		if (tempIndex == tempIndex + elementSize)
			m_ArrayStack.pop_back();

		m_Index = tempIndex;

		char *buffer = &m_Buffer[m_Index];

		m_Index += elementSize;

		return buffer;
	}
}