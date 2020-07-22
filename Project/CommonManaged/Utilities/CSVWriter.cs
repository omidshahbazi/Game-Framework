// Copyright 2019. All Rights Reserved.
using System.Data;
using System.Text;

namespace GameFramework.Common.Utilities
{
	public static class CSVWriter
	{
		public const char COMMA = ',';

		public static void Write(StringBuilder Builder, int OffsetX, int OffsetY, DataTable Data)
		{
			AddOffsetY(Builder, OffsetY);

			AddOffsetX(Builder, OffsetX);

			if (Data == null)
				return;

			for (int i = 0; i < Data.Columns.Count; ++i)
			{
				if (i != 0)
					Builder.Append(COMMA);

				Builder.Append(Data.Columns[i].Caption);
			}

			Builder.AppendLine();

			for (int i = 0; i < Data.Rows.Count; ++i)
				WriteLine(Builder, OffsetX, 0, Data.Rows[i].ItemArray);
		}

		public static void Write(StringBuilder Builder, int OffsetX, int OffsetY, params object[] Values)
		{
			AddOffsetX(Builder, OffsetX);
			AddOffsetY(Builder, OffsetY);

			if (Values == null)
				return;

			for (int i = 0; i < Values.Length; ++i)
			{
				if (i != 0)
					Builder.Append(COMMA);

				object value = Values[i];

				if (value == null)
					continue;

				if (value is string)
					Builder.Append('"');

				Builder.Append(value);

				if (value is string)
					Builder.Append('"');
			}
		}

		public static void WriteLine(StringBuilder Builder, int OffsetX, int OffsetY, params object[] Values)
		{
			Write(Builder, OffsetX, OffsetY, Values);

			Builder.AppendLine();
		}

		private static void AddOffsetX(StringBuilder Builder, int Offset)
		{
			for (int i = 0; i < Offset; ++i)
				Builder.Append(COMMA);
		}

		private static void AddOffsetY(StringBuilder Builder, int Offset)
		{
			for (int i = 0; i < Offset; ++i)
				Builder.AppendLine();
		}
	}
}