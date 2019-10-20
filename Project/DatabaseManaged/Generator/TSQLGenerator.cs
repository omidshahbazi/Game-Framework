// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Data;

namespace GameFramework.DatabaseManaged.Generator
{
	public enum SyncTypes
	{
		Keep,
		Delete
	}

	public class TSQLItem
	{
		public string Transact
		{
			get;
			private set;
		}

		public TSQLItem(string Transact)
		{
			this.Transact = Transact;
		}

		public void Execute(IConnection Connection)
		{
			Connection.Execute(Transact);
		}
	}

	public class MySqlGenerator
	{
		public static TSQLItem CreateDatabase(string DatabaseName)
		{
			return new TSQLItem("CREATE DATABASE " + DatabaseName);
		}

		public static TSQLItem DropDatabase(string DatabaseName)
		{
			return new TSQLItem("DROP DATABASE " + DatabaseName);
		}

		public static TSQLItem CreateTable(IConnection Connection, Table table, SyncTypes SyncType)
		{
			if (!Exists(Connection, table))
				return new TSQLItem("CREATE TABLE " + table.Name + "\n(\n" + WrapColumns(table) + AddIndex(table) + ")" + GetEngine(table) + GetCollate(table) + ";");

			Column[] cols = GetColumns(Connection, table);

			string str = string.Empty;

			AddToDatabase(Connection, table, cols, ref str);

			SyncToTable(table, SyncType, cols, ref str);

			//if (table.Collate != Collates.Server_Defualt)
			//	str += UpdateCollate(table).SqlString;

			Console.WriteLine(str);

			if (string.IsNullOrEmpty(str))
				return null;

			return new TSQLItem(str);
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

		private static void AddToDatabase(IConnection Connection, Table Table, Column[] Columns, ref string Transact)
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

		public static bool Exists(IConnection Connection, Table table)
		{
			TSQLItem sqlItem = new TSQLItem("SELECT 1 FROM " + table.Name + " LIMIT 1 ");
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

		private static bool Exists(IConnection Connection, Table table, Column column)
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

		public static TSQLItem AddColumn(IConnection Connection, Table table, Column column)
		{
			if (!Exists(Connection, table))
				return null;

			//if (!Exists(table, column))
			return new TSQLItem("ALTER TABLE " + table.Name + " ADD `" + column.Name + "` " + column.Type.ToString() + GetDefaultValue(column));

			//return new MySQLItem(ModifyColumn(table, column, column.Type).SqlString);
		}

		public static string FindToDropColumns(IConnection Connection)
		{
			string str = string.Empty;
			DataTable tables = Connection.ExecuteWithReturn("SHOW tables ");
			string colName = tables.Columns[0].ColumnName;
			foreach (DataRow row in tables.Rows)
			{
				DataTable tbl = Connection.ExecuteWithReturn("describe " + row[colName].ToString());

				for (int i = 0; i < tbl.Rows.Count; i++)
				{
					if (tbl.Rows[i]["Field"].ToString().Contains("_drop"))
						str += "ALTER TABLE " + row[colName].ToString() + " DROP `" + tbl.Rows[i]["Field"].ToString() + "`;\n";
				}
			}
			return str;
		}

		public static TSQLItem DropColumn(Table table, Column column, SQLType type)
		{
			return new TSQLItem("ALTER TABLE " + table.Name + " CHANGE `" + column.Name + "`  `" + column.Name + "_drop` " + type.ToString());

		}

		public static TSQLItem ModifyColumn(Table table, Column column, SQLType type, Constraints constriants)
		{
			return new TSQLItem("ALTER TABLE " + table.Name + " MODIFY `" + column.Name + "` " + type.ToString() + " " + GetFrontFlags(new Column("", DBType.Bit, constriants)));
		}

		public static TSQLItem UpdateCollate(IConnection Connection, Table table)
		{
			if (!Exists(Connection, table))
				return new TSQLItem(string.Empty);

			return new TSQLItem("ALTER TABLE " + table.Name + " COLLATE " + table.Collate + ";");
		}

		// TODO: Other EndFlags Defaults

		private static string GetEngine(Table Table)
		{
			return "\nENGINE=" + Table.Engine.ToString();
		}


		private static string GetCollate(Table Table)
		{
			string str = "\nDEFAULT CHARACTER SET =";

			switch (Table.Collate)
			{
				case Collates.Persian_CS_AS_KS_WS:
					str += "utf8";
					break;
				case Collates.Arabic_CS_AS_KS_WS:
					str += "utf8";
					break;
				case Collates.Latin1_General_CL_AS_KS_WS:
					str += "utf8";
					break;
				case Collates.Server_Defualt:
					str += "utf8";
					break;
				default:
					str += "utf8";
					break;
			}
			return str;
		}

		private static string AddIndex(Table Table)
		{
			string str = string.Empty;
			if (Table.Index != null)
			{
				for (int i = 0; i < Table.Index.IndexItem.Length; i++)
				{
					str += ", KEY " + Table.Index.IndexItem[i].Name + " (";

					for (int j = 0; j < Table.Index.IndexItem[i].Columns.Length; j++)
					{
						str += "`" + Table.Index.IndexItem[i].Columns[j] + "`";

						if (j == Table.Index.IndexItem[i].Columns.Length - 1)
							str += " ";
						else
							str += " , ";
					}
					str += " )\n";
				}
			}

			return str;
		}

		private static string WrapColumns(Table Table)
		{
			string str = string.Empty;

			string primaryKey = string.Empty;

			for (int i = 0; i < Table.Columns.Length; ++i)
			{
				Column column = Table.Columns[i];

				str += "`" + Table.Columns[i].Name + "` " + GetSqlTypeString(column);
				str += GetFrontFlags(column);
				str += GetDefaultValue(column);

				if (i < Table.Columns.Length - 1)
					str += ",\n";

				if (Contains(column.Flags, Constraints.PrimaryKey))
					primaryKey = column.Name;
			}

			if (primaryKey != string.Empty)
			{
				str += ",\n ";
				str += "PRIMARY KEY(" + primaryKey + ")\n";
			}

			return str;
		}

		private static Column[] GetColumns(IConnection Connection, Table Table)
		{
			DataTable tbl = Connection.ExecuteWithReturn("describe " + Table.Name);

			List<Column> cols = new List<Column>();

			//TODO :: Check Type | Constrains 
			for (int i = 0; i < tbl.Rows.Count; i++)
			{
				DataRow row = tbl.Rows[i];
				Column col = new Column(row["Field"].ToString(), GetSqlTypeFromType(row["Type"].ToString()));
				cols.Add(col);
			}

			return cols.ToArray();
		}

		private static string GetFrontFlags(Column col)
		{
			string flags = string.Empty;

			if (Contains(col.Flags, Constraints.NotNull))
				flags += " NOT NULL ";
			if (Contains(col.Flags, Constraints.AutoIncrement))
				flags += " AUTO_INCREMENT ";
			return flags;
		}

		private static string GetDefaultValue(Column col)
		{
			string defaultValue = string.Empty;

			if (col.DefaultValue != null)
			{
				defaultValue += " DEFAULT ";
				if (col.Type.Name == DBType.Int.Name)
					defaultValue += col.DefaultValue.ToString();
				else
					defaultValue += "'" + col.DefaultValue.ToString() + "'";
			}
			return defaultValue;
		}

		public static bool Contains(Constraints flags, Constraints flag)
		{
			return (flags & flag) == flag;
		}

		public static string GetSqlTypeString(Column column)
		{
			return column.Type.ToString();
		}

		// TODO : Other Types
		public static SQLType GetSqlTypeFromType(string type)
		{
			List<SQLType> types = DBType.Types;

			for (int i = 0; i < types.Count; i++)
			{
				if (type == types[i].ToString())
					return types[i];
			}
			return DBType.Text;
		}
	}
}

