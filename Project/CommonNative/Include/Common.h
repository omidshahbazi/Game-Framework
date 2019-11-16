// Copyright 2019. All Rights Reserved.
#pragma once

#ifdef COMMON_API
#undef COMMON_API
#endif

#ifdef BUILD_DLL
#define COMMON_API __declspec(dllexport)
#else
#define COMMON_API
#endif