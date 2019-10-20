// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.DatabaseManaged.Generator
{
    [Flags]
    public enum Constraints
    {
        NotNull = 1,
        AutoIncrement = 2,
        Unique = 4,
        PrimaryKey = 8,
        ForeignKey = 16,
        Default = 32,
    }
}
