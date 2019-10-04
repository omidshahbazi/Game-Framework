// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using System.Data;

namespace GameFramework.DatabaseManaged
{
	public abstract class Database
	{
		public abstract int LastInsertID
		{
			get;
		}

		public abstract void Execute(string Query, params object[] Parameters);

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
					string colName = Table.Columns[i].ColumnName;

					obj.Set(colName, row[colName]);
				}
			}

			return arr;
		}
	}
}
