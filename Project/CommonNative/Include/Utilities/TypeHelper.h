// Copyright 2019. All Rights Reserved.
#pragma once

#ifndef TYPE_HELPER_H
#define TYPE_HELPER_H

namespace GameFramework::Common::Utilities
{
#define IS_TYPE_OF(Pointer, DerivedType) (dynamic_cast<DerivedType*>(Pointer) != nullptr)
}

#endif