// Copyright 2019. All Rights Reserved.


using System.Collections.Generic;
using System.Reflection;

namespace GameFramework.DatabaseManaged.Generator
{
	public struct SQLType
	{
		private string name;
		private int lenght;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public int Lenght
		{
			get
			{
				return lenght;
			}
			set
			{
				lenght = value;
			}
		}

		public SQLType(string Name)
		{
			name = Name;
			lenght = 0;
		}

		public SQLType(string Name, int Lenght)
		{
			name = Name;
			lenght = Lenght;
		}

		public override string ToString()
		{
			return name + (lenght != -1 ? (lenght != 0 ? "(" + lenght.ToString() + ")" : "") : "(max)");
		}
	}

	public struct DBType
	{
		public static SQLType Bit
		{
			get
			{
				return new SQLType("bit");
			}

		}

		public static SQLType Binary
		{
			get
			{
				return new SQLType("binary", 50);
			}
		}

		public static SQLType VarBinary
		{
			get
			{
				return new SQLType("varbinary", 1000);
			}
		}

		public static SQLType VarBinaryMax
		{
			get
			{
				return new SQLType("varbinary", -1);
			}
		}

		public static SQLType Image
		{
			get
			{
				return new SQLType("image");
			}
		}

		public static SQLType Decimal
		{
			get
			{
				return new SQLType("decimal", 18);
			}
		}

		public static SQLType Float
		{
			get
			{
				return new SQLType("float");
			}
		}

		public static SQLType Int
		{
			get
			{
				return new SQLType("int", 11);
			}
		}

		public static SQLType BigInt
		{
			get
			{
				return new SQLType("bigint", 64);
			}
		}

		public static SQLType Numeric
		{
			get
			{
				return new SQLType("numeric", 18);
			}
		}

		public static SQLType Money
		{
			get
			{
				return new SQLType("money");
			}
		}

		public static SQLType DateTime
		{
			get
			{
				return new SQLType("datetime");
			}
		}

		public static SQLType Char40
		{
			get
			{
				return new SQLType("char", 40);
			}
		}

		public static SQLType VarChar6
		{
			get
			{
				return new SQLType("varchar", 6);
			}
		}

		public static SQLType VarChar45
		{
			get
			{
				return new SQLType("varchar", 45);
			}
		}

		public static SQLType VarChar100
		{
			get
			{
				return new SQLType("varchar", 100);
			}
		}

		public static SQLType NChar
		{
			get
			{
				return new SQLType("nchar", 10);
			}
		}

		public static SQLType NVarChar
		{
			get
			{
				return new SQLType("nvarchar", 50);
			}
		}

		public static SQLType Text
		{
			get
			{
				return new SQLType("TEXT");
			}
		}

		public static SQLType LongText
		{
			get
			{
				return new SQLType("longtext");
			}
		}


		public static List<SQLType> Types
		{
			get { return GetInsideMembers<SQLType>(); }
		}

		private static List<T> GetInsideMembers<T>() where T : struct
		{
			List<T> types = new List<T>();

			PropertyInfo[] fields = typeof(DBType).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

			for (int i = 0; i < fields.Length; ++i)
			{
				PropertyInfo field = fields[i];
				if (field.Name == "Types")
					continue;
				object value = field.GetValue(null, null);

				if (field.PropertyType != typeof(T))
					continue;

				types.Add((T)value);
			}

			return types;
		}

	}

}
