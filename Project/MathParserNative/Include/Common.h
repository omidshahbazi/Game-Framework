// Copyright 2019. All Rights Reserved.
#pragma once

#ifdef MATH_PARSER_API
#undef MATH_PARSER_API
#endif

#ifdef BUILD_DLL
#define MATH_PARSER_API __declspec(dllexport)
#else
#define MATH_PARSER_API
#endif