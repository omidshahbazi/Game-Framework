// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Utilities;
using GameFramework.DatabaseManaged;
using GameFramework.DatabaseManaged.Generator;
using System.Text;

namespace GameFramework.Analytics
{
	static class DatabaseGenerator
	{
		private static Table ResourcesFlowTable = new Table("resources_flow", Collates.UTF8, Engines.InnoDB, new Column("id", DataType.Int, Flags.PrimaryKey | Flags.AutoIncrement), new Column("user_id", DataType.Int), new Column("place", DataType.Int), new Column("resource_type", DataType.Int), new Column("flow_type", DataType.Int), new Column("amount", DataType.Int), new Column("progress", DataType.Int), new Column("occurs_time", DataType.DateTime));

		public static void UpdateStructure(Database Database)
		{
			Table[] tables = ReflectionExtensions.GetFields<Table>(typeof(DatabaseGenerator), ReflectionExtensions.PrivateStaticFlags);

			StringBuilder updateQuery = new StringBuilder();
			StringBuilder deprecatedQuery = new StringBuilder();

			for (int i = 0; i < tables.Length; ++i)
				TSQLGenerator.MySQL.GenerateCreateTable(Database, tables[i], SyncTypes.Keep, updateQuery, deprecatedQuery);

			if (updateQuery.Length != 0)
				Database.Execute(updateQuery.ToString());
		}
	}
}
