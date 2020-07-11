// Copyright 2019. All Rights Reserved.
namespace GameFramework.DatabaseManaged.Migration
{
	static class Constants
	{
		public const string MIGRATION_TABLE_NAME = "__migrations";

		private static class MySQL
		{
			public static readonly string[] MIGRATIONS = new string[] { MIGRATION_CORE_20200709 };
			public static readonly string[] MIGRATIONS_NAME = new string[] { "Migration_Core_20200709" };

			private const string MIGRATION_CORE_20200709 = "CREATE TABLE `" + MIGRATION_TABLE_NAME + "` (`name` TEXT NOT NULL, `apply_time` DATETIME NOT NULL);";
		}

		public static string[] GetMigrations(Database Database)
		{
			if (Database is MySQLDatabase)
				return MySQL.MIGRATIONS;

			return null;
		}

		public static string[] GetMigrationsName(Database Database)
		{
			if (Database is MySQLDatabase)
				return MySQL.MIGRATIONS_NAME;

			return null;
		}
	}
}