// Copyright 2019. All Rights Reserved.
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace GameFramework.DatabaseManaged
{
	public class MySQLDatabase : Database
	{
		private string connectionString;

		public MySQLDatabase(string Host, string Username, string Password) :
			this(Host, 3306, Username, Password, "")
		{
		}

		public MySQLDatabase(string Host, ushort Port, string Username, string Password) :
			this(Host, Port, Username, Password, "")
		{
		}

		public MySQLDatabase(string Host, string Username, string Password, string Name) :
			this(Host, 3306, Username, Password, Name)
		{
		}

		public MySQLDatabase(string Host, ushort Port, string Username, string Password, string Name)
		{
			CreateInfo info = new CreateInfo();
			info.Host = Host;
			info.Port = Port;
			info.Username = Username;
			info.Password = Password;
			info.Name = Name;
			info.PoolingEnabled = false;
			info.CharacterSet = CreateInfo.CharacterSets.UTF8;

			FillConnectionString(info);
		}

		public MySQLDatabase(CreateInfo Info)
		{
			FillConnectionString(Info);
		}

		public override void Close()
		{

		}

		public override void Execute(string Query, params object[] Parameters)
		{
			MySqlConnection connection = CreateConnection();

			MySqlCommand command = CreateCommand(connection, Query, Parameters);
			command.ExecuteNonQuery();
			command.Dispose();

			connection.Close();
			connection.Dispose();
		}

		public override int ExecuteInsert(string Query, params object[] Parameters)
		{
			MySqlConnection connection = CreateConnection();

			MySqlCommand command = CreateCommand(connection, Query, Parameters);
			command.ExecuteNonQuery();
			command.Dispose();

			int id = GetLastInsertID(command.Connection);

			connection.Close();
			connection.Dispose();

			return id;
		}

		public override DataTable ExecuteWithReturnDataTable(string Query, params object[] Parameters)
		{
			MySqlConnection connection = CreateConnection();

			DataTable table = ExecuteWithReturnDataTable(connection, Query, Parameters);

			connection.Close();
			connection.Dispose();

			return table;
		}

		private DataTable ExecuteWithReturnDataTable(MySqlConnection Connection, string Query, params object[] Parameters)
		{
			MySqlCommand command = CreateCommand(Connection, Query, Parameters);
			MySqlDataAdapter adapter = new MySqlDataAdapter(command);

			DataTable table = new DataTable();
			adapter.Fill(table);

			adapter.Dispose();
			command.Dispose();
			command.Cancel();

			return table;
		}

		private int GetLastInsertID(MySqlConnection Connection)
		{
			DataTable table = ExecuteWithReturnDataTable(Connection, "SELECT LAST_INSERT_ID() id");

			if (table == null || table.Rows.Count == 0)
				return 0;

			return Convert.ToInt32(table.Rows[0]["id"]);
		}

		private MySqlConnection CreateConnection()
		{
			MySqlConnection connection = new MySqlConnection(connectionString);

			connection.Open();

			return connection;
		}

		private MySqlCommand CreateCommand(string Query, object[] Parameters)
		{
			MySqlConnection connection = CreateConnection();

			return CreateCommand(connection, Query, Parameters);
		}

		private MySqlCommand CreateCommand(MySqlConnection Connection, string Query, object[] Parameters)
		{
			Debug.Assert(Parameters == null || Parameters.Length % 2 == 0, "Parameters count must be even");

			MySqlCommand command = new MySqlCommand(Query, Connection);
			command.CommandType = CommandType.Text;

			if (Parameters != null)
				for (int i = 0; i < Parameters.Length; i += 2)
					command.Parameters.Add(new MySqlParameter(Parameters[i].ToString(), Parameters[i + 1]));

			return command;
		}

		private void FillConnectionString(CreateInfo Info)
		{
			MySqlConnectionStringBuilder conStr = new MySqlConnectionStringBuilder();

			conStr.Server = Info.Host;
			conStr.Port = Info.Port;
			conStr.UserID = Info.Username;
			conStr.Password = Info.Password;

			conStr.Database = Info.Name;

			conStr.PersistSecurityInfo = true;

			conStr.CharacterSet = Info.CharacterSet.ToString().ToLower();

			conStr.Pooling = Info.PoolingEnabled;
			conStr.MinimumPoolSize = Info.MinimumPoolSize;
			conStr.MaximumPoolSize = Info.MaximumPoolSize;

			connectionString = conStr.GetConnectionString(true);
		}
	}
}