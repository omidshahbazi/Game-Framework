// Copyright 2019. All Rights Reserved.
#pragma once

#ifdef BINARY_SERIALIZE_API
#undef BINARY_SERIALIZE_API
#endif

#ifdef BUILD_DLL
#define BINARY_SERIALIZE_API __declspec(dllexport)
#else
#define BINARY_SERIALIZE_API
#endif