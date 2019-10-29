// Copyright 2019. All Rights Reserved.
//#define USING_POOL
using GameFramework.Common.Pool;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace GameFramework.DatabaseManaged
{
	public class MySQLDatabase : Database
	{
#if USING_POOL
		private class ConnectionHolder : IObject
		{
			public MySqlConnection Connection
			{
				get;
				private set;
			}

			public bool IsConnectionAlive
			{
				get { return !(Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken); }
			}

			public ConnectionHolder(string ConnectionString)
			{
				Connection = new MySqlConnection(ConnectionString);

				CheckConnection();
			}

			public void CheckConnection()
			{
				if (IsConnectionAlive)
					return;

				Connection.Open();

				if (!IsConnectionAlive)
					throw new Exception("Database connection is no alive");
			}

			void IObject.GoOutOfPool()
			{
				CheckConnection();
			}

			void IObject.GoInPool()
			{
			}
		}

		private class PoolFactory : IObjectFactory<ConnectionHolder>
		{
			private string connectionString;

			public PoolFactory(string ConnectionString, uint PoolCount)
			{
				connectionString = ConnectionString;

				for (uint i = 0; i < PoolCount; ++i)
					((IObjectFactory<ConnectionHolder>)this).Instantiate(null);
			}

			void IObjectFactory<ConnectionHolder>.AfterSendToPool(ConnectionHolder Object)
			{
			}

			void IObjectFactory<ConnectionHolder>.BeforeGetFromPool(ConnectionHolder Object, object UserData)
			{
			}

			void IObjectFactory<ConnectionHolder>.Destroy(ConnectionHolder Object)
			{
				Object.Connection.Dispose();
			}

			ConnectionHolder IObjectFactory<ConnectionHolder>.Instantiate(object UserData = null)
			{
				return new ConnectionHolder((string)connectionString);
			}
		}

		private class Pool : ObjectPool<ConnectionHolder>
		{
		}

		private Pool pool = null;
#else
		private MySqlConnection connection = null;
#endif

		public MySQLDatabase(string Host, string Username, string Password, uint PoolCount = 1) :
			this(Host, Username, Password, "", PoolCount)
		{
		}

		public MySQLDatabase(string Host, string Username, string Password, string Name, uint PoolCount = 1)
		{
			MySqlConnectionStringBuilder conStr = new MySqlConnectionStringBuilder();

			conStr.Server = Host;
			conStr.UserID = Username;
			conStr.Password = Password;

			conStr.Database = Name;

			conStr.PersistSecurityInfo = true;

			conStr.CharacterSet = "utf8";

			string connectionString = conStr.GetConnectionString(true);

#if USING_POOL
			pool = new Pool();
			pool.Factory = new PoolFactory(connectionString, PoolCount);
#else
			connection = new MySqlConnection(connectionString);

			CheckConnection();
#endif
		}

		public override void Execute(string Query, params object[] Parameters)
		{
#if USING_POOL
			while (true)
			{
				try
				{
					ConnectionHolder con = pool.Pull();

					MySqlCommand command = CreateCommand(con.Connection, Query, Parameters);
					command.ExecuteNonQuery();
					command.Dispose();

					pool.Push(con);

					break;
				}
				catch
				{
				}
			}
#else
			CheckConnection();

			MySqlCommand command = CreateCommand(connection, Query, Parameters);
			command.ExecuteNonQuery();
			command.Dispose();
#endif
		}

		public override int ExecuteInsert(string Query, params object[] Parameters)
		{
#if USING_POOL
			while (true)
			{
				try
				{
					ConnectionHolder con = pool.Pull();

					MySqlCommand command = CreateCommand(con.Connection, Query, Parameters);
					command.ExecuteNonQuery();
					command.Dispose();

					int id = GetLastInsertID(con.Connection);

					pool.Push(con);

					return id;
				}
				catch
				{
				}
			}
#else
			CheckConnection();

			MySqlCommand command = CreateCommand(connection, Query, Parameters);
			command.ExecuteNonQuery();
			command.Dispose();

			return GetLastInsertID(connection);
#endif
		}

		public override DataTable ExecuteWithReturnDataTable(string Query, params object[] Parameters)
		{
#if USING_POOL
			while (true)
			{
				try
				{
					ConnectionHolder con = pool.Pull();

					DataTable table = ExecuteWithReturnDataTable(con.Connection, Query, Parameters);

					pool.Push(con);

					return table;
				}
				catch
				{
				}
			}
#else
			CheckConnection();

			return ExecuteWithReturnDataTable(connection, Query, Parameters);
#endif
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

#if !USING_POOL
		public bool IsConnectionAlive
		{
			get { return !(connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken); }
		}

		public void CheckConnection()
		{
			if (IsConnectionAlive)
				return;

			connection.Open();

			if (!IsConnectionAlive)
				throw new Exception("Database connection is no alive");
		}
#endif
	}
}