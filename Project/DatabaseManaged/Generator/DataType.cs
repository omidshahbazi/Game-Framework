// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public struct DataType
	{
		public static readonly DataType Bit = new DataType("BIT");
		public static readonly DataType Binary = new DataType("BINARY");
		public static readonly DataType VarBinaryMax = new DataType("VARBINARY", -1);
		public static readonly DataType Float = new DataType("FLOAT");
		public static readonly DataType Int = new DataType("INT", 11);
		public static readonly DataType BigInt = new DataType("BIGINT", 64);
		public static readonly DataType DateTime = new DataType("DATETIME");
		public static readonly DataType VarChar = new DataType("VARCHAR", 45);
		public static readonly DataType NVarChar = new DataType("NVARCHAR", 45);
		public static readonly DataType Text = new DataType("TEXT");
		public static readonly DataType LongText = new DataType("LONTTEXT");

		public string Name { get; set; }
		public int Lenght { get; set; }

		public DataType(string Name)
		{
			this.Name = Name;
			Lenght = 0;
		}

		public DataType(string Name, int Lenght)
		{
			this.Name = Name;
			this.Lenght = Lenght;
		}

		public static DataType[] Types
		{
			get { return ReflectionExtensions.GetFields<DataType>(typeof(DataType)); }
		}
	}
}