// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GameFramework.DatabaseManaged.Generator
{
	public static class TSQLGenerator
	{
		private const string DEPRECATED_POST_FIX = "_deprecated";

		public static class MySQL
		{
			public static void GenerateCreateCatalog(Database Database, Catalog Catalog, SyncTypes SyncType, StringBuilder Builder, StringBuilder KeepSyncTypeBuilder = null)
			{
				if (!IsCatalogExists(Database, Catalog))
				{
					Builder.Append("CREATE DATABASE ");
					Builder.Append(Catalog.Name);
					Builder.Append(';');
					Builder.AppendLine();
				}

				Builder.Append("USE ");
				Builder.Append(Catalog.Name);
				Builder.Append(';');
				Builder.AppendLine();

				for (int i = 0; i < Catalog.Tables.Length; ++i)
					GenerateCreateTable(Database, Catalog.Tables[i], SyncType, Builder, KeepSyncTypeBuilder);
			}

			public static void GenerateDropCatalog(Catalog Catalog, StringBuilder Builder)
			{
				Builder.Append("DROP DATABASE ");
				Builder.Append(Catalog.Name);
				Builder.AppendLine();
			}

			public static void GenerateCreateTable(Database Database, Table Table, SyncTypes SyncType, StringBuilder Builder, StringBuilder KeepSyncTypeBuilder = null)
			{
				if (!IsTableExists(Database, Table))
				{
					Builder.Append("CREATE TABLE `");
					Builder.Append(Table.Name);
					Builder.Append('`');
					Builder.AppendLine();
					Builder.Append("(");

					string primaryKey = "";

					for (int i = 0; i < Table.Columns.Length; ++i)
					{
						if (i != 0)
						{
							Builder.Append(',');
							Builder.AppendLine();
						}

						Column column = Table.Columns[i];

						Builder.Append("`");
						Builder.Append(column.Name);
						Builder.Append("` ");

						GenerateDataType(column.DataType, Builder);

						GeneratePreFlags(column.FlagMask, Builder);

						GenerateDefaultValue(column, Builder);

						if (MaskContainsFlag(column.FlagMask, Flags.PrimaryKey))
							primaryKey = column.Name;
					}

					if (!string.IsNullOrEmpty(primaryKey))
					{
						Builder.Append(',');
						Builder.AppendLine();

						Builder.Append("PRIMARY KEY(`");
						Builder.Append(primaryKey);
						Builder.Append("`)");
						Builder.AppendLine();
					}

					GenerateCreateIndexGroup(Table.IndexGroup, Builder);

					Builder.Append(')');

					Builder.Append("ENGINE=");
					Builder.Append(Table.Engine);
					Builder.Append(' ');

					GenerateCollate(Table.Collate, Builder);

					Builder.Append(';');
					Builder.AppendLine();

					return;
				}

				Column[] oldColumns = GetColumns(Database, Table.Name);
				Index[] oldIndecies = GetIndecies(Database, Table.Name);

				GenerateNewColumnsAdd(Database, Table, oldColumns, Builder);
				GenerateDeprecatedColumns(Table, SyncType, oldColumns, oldIndecies, Builder, KeepSyncTypeBuilder);
			}

			public static void GenerateDropTable(string Name, StringBuilder Builder)
			{
				Builder.Append("DROP TABLE IF EXISTS ");
				Builder.Append(Name);
				Builder.Append(';');
			}

			public static void GenerateCreateColumn(Database Database, Table Table, Column Column, Column PrevColumn, StringBuilder Builder)
			{
				if (IsColumnExists(Database, Table, Column))
					return;

				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` ADD `");
				Builder.Append(Column.Name);
				Builder.Append("` ");

				GenerateDataType(Column.DataType, Builder);

				GeneratePreFlags(Column.FlagMask, Builder);

				GenerateDefaultValue(Column, Builder);

				if (PrevColumn != null)
				{
					Builder.Append(" AFTER `");
					Builder.Append(PrevColumn.Name);
					Builder.Append('`');
				}

				Builder.Append(';');
			}

			public static void GenerateDropColumn(Table Table, Column Column, StringBuilder Builder)
			{
				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` DROP COLUMN `");
				Builder.Append(Column.Name);
				Builder.Append("`;");
				Builder.AppendLine();
			}

			public static void GenerateDeprecatedColumn(Table Table, Column Column, StringBuilder Builder, StringBuilder KeepSyncTypeBuilder)
			{
				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` CHANGE `");
				Builder.Append(Column.Name);
				Builder.Append("` `");
				Builder.Append(GetDeprecatedColumnName(Column.Name));
				Builder.Append("` ");

				GenerateDataType(Column.DataType, Builder);

				Builder.Append(';');
				Builder.AppendLine();

				GenerateDropColumn(Table, Column, KeepSyncTypeBuilder);
			}

			public static void GenerateModifyColumn(Table Table, Column OldColumn, Column NewColumn, StringBuilder Builder)
			{
				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` MODIFY `");
				Builder.Append(OldColumn.Name);
				Builder.Append("` ");

				GenerateDataType(NewColumn.DataType, Builder);

				Builder.Append(' ');

				GeneratePreFlags(NewColumn.FlagMask, Builder);

				Builder.Append(';');
				Builder.AppendLine();
			}

			public static Column[] GetColumns(Database Database, string TableName)
			{
				DataTable table = Database.ExecuteWithReturnDataTable("DESCRIBE `" + TableName + "`");

				List<Column> columns = new List<Column>();

				for (int i = 0; i < table.Rows.Count; i++)
				{
					DataRow row = table.Rows[i];

					columns.Add(new Column(row["Field"].ToString(), GetDataType(row["Type"].ToString())));
				}

				return columns.ToArray();
			}

			public static Index[] GetIndecies(Database Database, string TableName)
			{
				DataTable table = Database.ExecuteWithReturnDataTable("SHOW INDEX FROM `" + TableName + "`");

				DataTable nameTable = table.DefaultView.ToTable(true, "Key_name");

				List<Index> indecies = new List<Index>();

				for (int i = 0; i < nameTable.Rows.Count; i++)
				{
					DataRow row = nameTable.Rows[i];

					string name = row["Key_name"].ToString();

					Index index = new Index(name);

					table.DefaultView.RowFilter = "Key_name='" + name + "'";
					for (int j = 0; j < table.DefaultView.Count; ++j)
					{
						DataRowView columnRow = table.DefaultView[j];

						index.AddColumn(columnRow["Column_name"].ToString());
					}

					indecies.Add(index);
				}

				return indecies.ToArray();
			}

			private static void GenerateCreateIndexGroup(IndexGroup IndexGroup, StringBuilder Builder)
			{
				if (IndexGroup == null)
					return;

				for (int i = 0; i < IndexGroup.Indecies.Length; i++)
				{
					Builder.Append(',');

					Index index = IndexGroup.Indecies[i];

					Builder.Append("KEY ");
					Builder.Append(index.Name);
					Builder.Append(" (");

					for (int j = 0; j < index.Columns.Length; j++)
					{
						if (j != 0)
							Builder.Append(',');

						Builder.Append('`');
						Builder.Append(index.Columns[j]);
						Builder.Append('`');
					}

					Builder.Append(')');

					Builder.AppendLine();
				}
			}

			private static void GenerateCreateIndex(Table Table, Index Index, StringBuilder Builder)
			{
				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` ADD INDEX `");
				Builder.Append(Index.Name);
				Builder.Append("` (");

				for (int j = 0; j < Index.Columns.Length; j++)
				{
					if (j != 0)
						Builder.Append(',');

					Builder.Append('`');
					Builder.Append(Index.Columns[j]);
					Builder.Append('`');
				}

				Builder.Append(");");
				Builder.AppendLine();
			}

			private static void GenerateDropInex(Table Table, Index Index, StringBuilder Builder)
			{
				Builder.Append("ALTER TABLE `");
				Builder.Append(Table.Name);
				Builder.Append("` DROP INDEX `");
				Builder.Append(Index.Name);
				Builder.Append("`;");
				Builder.AppendLine();
			}

			public static void GeneratePreFlags(Flags FlagMask, StringBuilder Builder)
			{
				if (MaskContainsFlag(FlagMask, Flags.NotNull))
					Builder.Append(" NOT NULL ");

				if (MaskContainsFlag(FlagMask, Flags.AutoIncrement))
					Builder.Append(" AUTO_INCREMENT ");
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

			public static void GenerateDefaultValue(Column Column, StringBuilder Builder)
			{
				if (Column.DefaultValue == null)
					return;

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

			public static bool IsCatalogExists(Database Database, Catalog Catalog)
			{
				try
				{
					Database.Execute("USE " + Catalog.Name + ";");

					return true;
				}
				catch
				{
				}

				return false;
			}

			public static bool IsTableExists(Database Database, Table Table)
			{
				try
				{
					Database.Execute("SELECT 1 FROM `" + Table.Name + "` LIMIT 1;");

					return true;
				}
				catch
				{
				}

				return false;
			}

			public static bool IsColumnExists(Database Database, Table Table, Column Column)
			{
				try
				{
					Database.Execute("SELECT `" + Column.Name + "` FROM `" + Table.Name + "` LIMIT 1");

					return true;
				}
				catch
				{
				}

				return false;
			}

			public static DataType GetDataType(string Type)
			{
				DataType[] types = DataType.Types;

				for (int i = 0; i < types.Length; ++i)
				{
					StringBuilder builder = new StringBuilder();
					GenerateDataType(types[i], builder);

					if (Type.ToUpper() == builder.ToString().ToUpper())
						return types[i];
				}

				return DataType.Text;
			}

			private static bool MaskContainsFlag(Flags FlagMask, Flags Flag)
			{
				return (FlagMask & Flag) == Flag;
			}

			private static void GenerateDeprecatedColumns(Table Table, SyncTypes SyncType, Column[] OldColumns, Index[] OldIndecies, StringBuilder Builder, StringBuilder KeepSyncTypeBuilder = null)
			{
				List<Index> toModifyIndecies = new List<Index>();
				for (int i = 0; i < OldColumns.Length; ++i)
				{
					Column oldColumn = OldColumns[i];

					if (SyncType == SyncTypes.Keep && oldColumn.Name.EndsWith(DEPRECATED_POST_FIX))
						continue;

					bool contains = false;
					for (int j = 0; j < Table.Columns.Length; ++j)
					{
						Column newColumn = Table.Columns[j];

						if (newColumn.Name != oldColumn.Name)
							continue;

						contains = true;
						break;
					}

					if (contains)
						continue;

					for (int j = 0; j < OldIndecies.Length; ++j)
					{
						Index index = OldIndecies[j];

						if (Array.IndexOf(index.Columns, oldColumn.Name) == -1)
							continue;

						index.RemmoveColumn(oldColumn.Name);

						if (SyncType == SyncTypes.Keep)
							index.AddColumn(GetDeprecatedColumnName(oldColumn.Name));

						toModifyIndecies.Add(index);

						GenerateDropInex(Table, index, Builder);
					}

					switch (SyncType)
					{
						case SyncTypes.Delete:
							GenerateDropColumn(Table, oldColumn, Builder);
							Builder.AppendLine();
							break;

						case SyncTypes.Keep:
							GenerateDeprecatedColumn(Table, oldColumn, Builder, KeepSyncTypeBuilder);
							Builder.AppendLine();
							break;
					}
				}

				if (Table.IndexGroup != null)
				{
					for (int i = 0; i < Table.IndexGroup.Indecies.Length; ++i)
					{
						Index index = Table.IndexGroup.Indecies[i];

						bool found = false;
						for (int j = 0; j < OldIndecies.Length; ++j)
						{
							if (index.Name != OldIndecies[j].Name)
								continue;

							found = true;
							break;
						}

						if (found)
							continue;

						toModifyIndecies.Add(index);
					}
				}

				for (int i = 0; i < toModifyIndecies.Count; ++i)
				{
					Index index = toModifyIndecies[i];

					if (index.Columns.Length == 0)
						continue;

					GenerateCreateIndex(Table, index, Builder);
				}
			}

			private static void GenerateNewColumnsAdd(Database Database, Table Table, Column[] OldColumns, StringBuilder Builder)
			{
				for (int i = 0; i < Table.Columns.Length; ++i)
				{
					Column newColumn = Table.Columns[i];

					bool contains = false;
					for (int j = 0; j < OldColumns.Length; ++j)
					{
						Column oldColumn = OldColumns[j];

						if (oldColumn.Name != newColumn.Name)
							continue;

						contains = true;

						if (oldColumn.DataType.Name != newColumn.DataType.Name)
						{
							GenerateModifyColumn(Table, oldColumn, newColumn, Builder);

							Builder.AppendLine();
						}

						break;
					}

					if (contains)
						continue;

					GenerateCreateColumn(Database, Table, newColumn, (i == 0 ? null : Table.Columns[i - 1]), Builder);

					Builder.AppendLine();
				}
			}

			private static string GetDeprecatedColumnName(string ColumnName)
			{
				return ColumnName + DEPRECATED_POST_FIX;
			}
		}
	}
}

