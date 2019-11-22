// Copyright 2019. All Rights Reserved.
#pragma once

#include <vector>

#ifdef NETWORKING_API
#undef NETWORKING_API
#endif

#ifdef BUILD_DLL
#define NETWORKING_API __declspec(dllexport)
#else
#define NETWORKING_API
#endif

template<typename T>
using List = std::vector<T>;