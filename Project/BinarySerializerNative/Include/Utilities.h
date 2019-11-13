// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef UTILITIES_H
#define UTILITIES_H

#include <cstddef>

using namespace std;

namespace GameFramework::BinarySerializer
{
	template<typename T>
	union BytesOf
	{
	public:
		T Value;
		byte Bytes[sizeof(T)];
	};
}

#endif