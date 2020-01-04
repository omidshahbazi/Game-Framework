// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using System;
using System.Data;

namespace GameFramework.DatabaseManaged
{
	public abstract class Database
	{
		public struct CreateInfo
		{
			public enum CharacterSets
			{
				ASCII = 0,
				UTF8 = 1
			}

			public string Host;
			public ushort Port;

			public string Username;
			public string Password;

			public string Name;

			public CharacterSets CharacterSet;

			public bool PoolingEnabled;
			public uint MinimumPoolSize;
			public uint MaximumPoolSize;
		}

		public abstract void Close();

		public abstract void Execute(string Query, params object[] Parameters);

		public abstract int ExecuteInsert(string Query, params object[] Parameters);

		public abstract DataTable ExecuteWithReturnDataTable(string Query, params object[] Parameters);

		public virtual ISerializeArray ExecuteWithReturnISerializeArray(string Query, params object[] Parameters)
		{
			return BuildISerializeData(ExecuteWithReturnDataTable(Query, Parameters));
		}

		protected ISerializeArray BuildISerializeData(DataTable Table)
		{
			if (Table == null || Table.Rows.Count == 0)
				return null;

			ISerializeArray arr = Creator.Create<ISerializeArray>();

			for (int i = 0; i < Table.Rows.Count; ++i)
			{
				DataRow row = Table.Rows[i];

				ISerializeObject obj = arr.AddObject();

				for (int j = 0; j < Table.Columns.Count; ++j)
				{
					string colName = Table.Columns[j].ColumnName;

					object value = row[colName];

					if (value == DBNull.Value)
						value = null;

					obj.Set(colName, value);
				}
			}

			return arr;
		}
	}
}
