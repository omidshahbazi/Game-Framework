// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GameFramework.DatabaseManaged.Generator
{
	public static class Generator
	{
		public static class MySQL
		{
			public static void GenerateCreateCatalog(Database Database, Catalog Catalog, SyncTypes SyncType, StringBuilder Builder)
			{
				Builder.Append("CREATE DATABASE ");
				Builder.Append(Catalog.Name);
				Builder.Append(';');
				Builder.AppendLine();

				for (int i = 0; i < Catalog.Tables.Length; ++i)
					GenerateCreateTable(Database, Catalog.Tables[i], SyncType, Builder);
			}

			public static void GenerateDropCatalog(Catalog Catalog, StringBuilder Builder)
			{
				Builder.Append("DROP DATABASE ");
				Builder.Append(Catalog.Name);
				Builder.Append(';');
			}

			public static void GenerateCreateTable(Database Database, Table Table, SyncTypes SyncType, StringBuilder Builder)
			{
				if (!IsTableExists(Database, Table))
				{
					Builder.Append("CREATE TABLE ");
					Builder.Append(Table.Name);
					Builder.AppendLine();
					Builder.Append('(');

					string primaryKey = "";

					for (int i = 0; i < Table.Columns.Length; ++i)
					{
						if (i != 0)
						{
							Builder.Append(',');
							Builder.AppendLine();
						}

						Column column = Table.Columns[i];

						GenerateCreateColumn(column, Builder);

						if (MaskContainsFlag(column.FlagMask, Flags.PrimaryKey))
							primaryKey = column.Name;
					}

					if (!string.IsNullOrEmpty(primaryKey))
					{
						Builder.Append(',');
						Builder.AppendLine();

						Builder.Append("PRIMARY KEY(");
						Builder.Append(primaryKey);
						Builder.Append(')');
						Builder.AppendLine();
					}

					GenerateCreateIndexGroup(Table.IndexGroup, Builder);

					Builder.Append(')');

					Builder.Append("ENGINE=");
					Builder.Append(Table.Engine);
					Builder.Append(' ');

					GenerateCollate(Table.Collate, Builder);

					Builder.Append(';');

					return;
				}

				Column[] columns = GetColumns(Database, Table.Name);

				AddToDatabase(Database, table, cols, ref str);

				SyncToTable(table, SyncType, cols, ref str);

				if (string.IsNullOrEmpty(str))
					return null;

				return new TSQLItem(str);
			}

			public static void GenerateCreateColumn(Column Column, StringBuilder Builder)
			{
				Builder.Append("`");
				Builder.Append(Column.Name);
				Builder.Append("` ");

				GenerateDataType(Column.DataType, Builder);

				if (MaskContainsFlag(Column.FlagMask, Flags.NotNull))
					Builder.Append(" NOT NULL ");
				if (MaskContainsFlag(Column.FlagMask, Flags.AutoIncrement))
					Builder.Append(" AUTO_INCREMENT ");

				if (Column.DefaultValue != null)
				{
					Builder.Append(" DEFAULT ");

					if (Column.DataType.Name == DataType.Int.Name)
						Builder.Append(Column.DefaultValue);
					else
					{
						Builder.Append('\'');
						Builder.Append(Column.DefaultValue);
						Builder.Append('\'');
					}
				}
			}

			public static Column[] GetColumns(Database Database, string TableName)
			{
				DataTable table = Database.ExecuteWithReturnDataTable("describe " + TableName);

				List<Column> columns = new List<Column>();

				for (int i = 0; i < table.Rows.Count; i++)
				{
					DataRow row = table.Rows[i];

					columns.Add(new Column(row["Field"].ToString(), GetDataType(row["Type"].ToString())));
				}

				return columns.ToArray();
			}

			private static void GenerateCreateIndexGroup(IndexGroup IndexGroup, StringBuilder Builder)
			{
				if (IndexGroup == null)
					return;

				for (int i = 0; i < IndexGroup.Indecies.Length; i++)
				{
					if (i != 0)
						Builder.Append(',');

					Index index = IndexGroup.Indecies[i];

					GenerateCreateIndex(index, Builder);

					Builder.AppendLine();
				}
			}

			private static void GenerateCreateIndex(Index Index, StringBuilder Builder)
			{
				Builder.Append("KEY ");
				Builder.Append(Index.Name);
				Builder.Append(" (");

				for (int i = 0; i < Index.Columns.Length; i++)
				{
					if (i != 0)
						Builder.Append(',');

					Builder.Append('`');
					Builder.Append(Index.Columns[i]);
					Builder.Append('`');
				}

				Builder.Append(')');
			}

			public static void GenerateDataType(DataType DataType, StringBuilder Builder)
			{
				Builder.Append(DataType.Name.ToUpper());

				if (DataType.Lenght == -1)
					Builder.Append("(MAX)");
				else if (DataType.Lenght != 0)
				{
					Builder.Append('(');
					Builder.Append(DataType.Lenght);
					Builder.Append(')');
				}
			}

			private static void GenerateCollate(Collates Collate, StringBuilder Builder)
			{
				Builder.Append("DEFAULT CHARACTER SET =");

				switch (Collate)
				{
					case Collates.Defualt:
						Builder.Append("utf8");
						break;
					case Collates.UTF8:
						Builder.Append("utf8");
						break;
					case Collates.ASCII:
						Builder.Append("utf8");
						break;
				}
			}

			public static bool IsTableExists(Database Database, Table Table)
			{
				try
				{
					Database.Execute("SELECT 1 FROM " + Table.Name + " LIMIT 1;");

					return true;
				}
				catch
				{
				}

				return false;
			}

			public static DataType GetDataType(string Type)
			{
				List<DataType> types = DataType.Types;

				for (int i = 0; i < types.Count; ++i)
				{
					if (Type.ToUpper() == types[i].ToString().ToUpper())
						return types[i];
				}

				return DataType.Text;
			}

			private static bool MaskContainsFlag(Flags FlagMask, Flags Flag)
			{
				return (FlagMask & Flag) == Flag;
			}

			private static void SyncToTable(Table Table, SyncTypes SyncType, Column[] Columns, ref string Transact)
			{
				for (int i = 0; i < Columns.Length; ++i)
				{
					Column serverCol = Columns[i];

					bool contains = false;

					for (int j = 0; j < Table.Columns.Length; ++j)
					{
						Column offlineCol = Table.Columns[j];
						if (offlineCol.Name == serverCol.Name)
						{
							contains = true;
						}
					}

					if (!contains)
					{
						switch (SyncType)
						{
							case SyncTypes.Delete:
								Transact += DropColumn(Table, serverCol, serverCol.Type).Transact + ";\n";
								break;
							case SyncTypes.Keep:
								break;
							default:
								break;
						}
					}
				}
			}

			private static void AddToDatabase(Database Database, Table Table, Column[] Columns, ref string Transact)
			{
				for (int i = 0; i < Table.Columns.Length; ++i)
				{
					Column offlineCol = Table.Columns[i];

					bool contains = false;

					for (int j = 0; j < Columns.Length; ++j)
					{
						Column serverCol = Columns[j];

						//TODO :: Check Type | Constrains 
						if (serverCol.Name == offlineCol.Name)
						{
							contains = true;
							if (serverCol.Type.Name != offlineCol.Type.Name)
							{
								Transact += ModifyColumn(Table, serverCol, offlineCol.Type, offlineCol.Flags).Transact + ";\n";
							}
						}
					}

					if (!contains)
					{
						Transact += AddColumn(Connection, Table, offlineCol).Transact + ";\n";
					}
				}
			}

			private static bool Exists(Database Database, Table table, Column column)
			{
				TSQLItem sqlItem = new TSQLItem("SELECT `" + column.Name + "` FROM " + table.Name + " LIMIT 1 ");
				try
				{
					sqlItem.Execute(Connection);

					return true;
				}
				catch
				{
				}

				return false;
			}

			public static TSQLItem DropTable(string name)
			{
				return new TSQLItem("DROP TABLE IF EXISTS " + name);
			}

			public static TSQLItem AddColumn(Database Database, Table table, Column column)
			{
				if (!Exists(Connection, table))
					return null;

				//if (!Exists(table, column))
				return new TSQLItem("ALTER TABLE " + table.Name + " ADD `" + column.Name + "` " + column.Type.ToString() + GetDefaultValue(column));

				//return new MySQLItem(ModifyColumn(table, column, column.Type).SqlString);
			}

			public static string FindToDropColumns(Database Database)
			{
				string str = string.Empty;
				DataTable tables = Database.ExecuteWithReturn("SHOW tables ");
				string colName = tables.Columns[0].ColumnName;
				foreach (DataRow row in tables.Rows)
				{
					DataTable tbl = Database.ExecuteWithReturn("describe " + row[colName].ToString());

					for (int i = 0; i < tbl.Rows.Count; i++)
					{
						if (tbl.Rows[i]["Field"].ToString().Contains("_drop"))
							str += "ALTER TABLE " + row[colName].ToString() + " DROP `" + tbl.Rows[i]["Field"].ToString() + "`;\n";
					}
				}
				return str;
			}

			public static TSQLItem DropColumn(Table table, Column column, DataType type)
			{
				return new TSQLItem("ALTER TABLE " + table.Name + " CHANGE `" + column.Name + "`  `" + column.Name + "_drop` " + type.ToString());

			}

			public static TSQLItem ModifyColumn(Table table, Column column, DataType type, Constraints constriants)
			{
				return new TSQLItem("ALTER TABLE " + table.Name + " MODIFY `" + column.Name + "` " + type.ToString() + " " + GetFrontFlags(new Column("", DBType.Bit, constriants)));
			}

			public static TSQLItem UpdateCollate(Database Database, Table table)
			{
				if (!Exists(Connection, table))
					return new TSQLItem(string.Empty);

				return new TSQLItem("ALTER TABLE " + table.Name + " COLLATE " + table.Collate + ";");
			}


		}
	}
}

