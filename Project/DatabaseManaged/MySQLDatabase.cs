// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
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

		public MySQLDatabase(string Host, string Username, string Password, string Name)
		{
			connection = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3}", Host, Username, Password, Name));

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
