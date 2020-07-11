// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.DatabaseManaged.Migration
{
	public static class MigrationManager
	{
		public static void Migrate(Database Database, string[] MigrationsName, Func<string, string> LoadCallback)
		{
			MigrateCore(Database);

			DoMigrate(Database, MigrationsName, LoadCallback);
		}

		private static void DoMigrate(Database Database, string[] MigrationsName, Func<string, string> LoadCallback)
		{
			if (MigrationsName == null)
				return;

			for (int i = 0; i < MigrationsName.Length; ++i)
			{
				string name = MigrationsName[i];

				if (Database.QueryDataTable("SHOW TABLES LIKE '" + Constants.MIGRATION_TABLE_NAME + "'").Rows.Count != 0 &&
					Database.QueryDataTable("SELECT 1 FROM `" + Constants.MIGRATION_TABLE_NAME + "` WHERE name=@name LIMIT 1", "name", name).Rows.Count != 0)
					continue;

				if (LoadCallback == null)
					throw new Exception("LoadCallback cannot be null");

				string migration = LoadCallback(name);
				if (migration == null)
					throw new Exception("Migration [" + name + "] not found");

				Database.Execute(migration);

				Database.Execute("INSERT INTO `" + Constants.MIGRATION_TABLE_NAME + "`(name, apply_time) VALUES(@name, NOW())", "name", name);
			}
		}

		private static void MigrateCore(Database Database)
		{
			string[] migrationsName = Constants.GetMigrationsName(Database);
			string[] migrations = Constants.GetMigrations(Database);

			Func<string, string> loadMigration = (string Name) =>
			{
				int index = Array.IndexOf(migrationsName, Name);
				if (index == -1)
					return null;

				if (index >= migrations.Length)
					return null;

				return migrations[index];
			};

			DoMigrate(Database, migrationsName, loadMigration);
		}
	}
}