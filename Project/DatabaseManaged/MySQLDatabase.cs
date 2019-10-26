// Copyright 2019. All Rights Reserved.
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace GameFramework.DatabaseManaged
{
	public class MySQLDatabase : Database
	{
		private MySqlConnection connection = null;

		public override int LastInsertID
		{
			get
			{
				DataTable table = ExecuteWithReturnDataTable("SELECT LAST_INSERT_ID() id");

				if (table == null || table.Rows.Count == 0)
					return 0;

				return Convert.ToInt32(table.Rows[0]["id"]);
			}
		}

		public MySQLDatabase(string Host, string Username, string Password, bool UsePool = false) :
			this(Host, Username, Password, "", UsePool)
		{
		}

		public MySQLDatabase(string Host, string Username, string Password, string Name, bool UsePool = false)
		{
			MySqlConnectionStringBuilder conStr = new MySqlConnectionStringBuilder();

			conStr.Server = Host;
			conStr.UserID = Username;
			conStr.Password = Password;

			conStr.Database = Name;

			if (UsePool)
			{
				conStr.Pooling = true;
				conStr.MinimumPoolSize = 20;
				conStr.MaximumPoolSize = 100;
			}

			conStr.PersistSecurityInfo = true;

			conStr.CharacterSet = "utf8";

			connection = new MySqlConnection(conStr.GetConnectionString(true));

			CheckConnection();
		}

		public override void Execute(string Query, params object[] Parameters)
		{
			CheckConnection();

			CreateCommand(Query, Parameters).ExecuteNonQuery();
		}

		public override DataTable ExecuteWithReturnDataTable(string Query, params object[] Parameters)
		{
			CheckConnection();

			MySqlDataAdapter adapter = new MySqlDataAdapter(CreateCommand(Query, Parameters));

			DataTable table = new DataTable();
			adapter.Fill(table);

			return table;
		}

		private MySqlCommand CreateCommand(string Query, object[] Parameters)
		{
			Debug.Assert(Parameters == null || Parameters.Length % 2 == 0, "Parameters count must be even");

			MySqlCommand command = new MySqlCommand(Query, connection);
			command.CommandType = CommandType.Text;

			if (Parameters != null)
				for (int i = 0; i < Parameters.Length; i += 2)
					command.Parameters.Add(new MySqlParameter(Parameters[i].ToString(), Parameters[i + 1]));

			return command;
		}

		private void CheckConnection()
		{
			if (IsConnectionAlive())
				return;

			connection.Open();

			if (!IsConnectionAlive())
				throw new Exception("Database connection is no alive");
		}

		private bool IsConnectionAlive()
		{
			return !(connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken);
		}
	}
}