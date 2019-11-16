// Copyright 2019. All Rights Reserved.
#pragma once

#ifdef NETWORKING_API
#undef NETWORKING_API
#endif

#ifdef BUILD_DLL
#define NETWORKING_API __declspec(dllexport)
#else
#define NETWORKING_API
#endif