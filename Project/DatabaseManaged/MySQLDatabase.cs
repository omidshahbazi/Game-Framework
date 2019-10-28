// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Pool;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace GameFramework.DatabaseManaged
{
	public class MySQLDatabase : Database
	{
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

		private class Pool : ObjectPool<ConnectionHolder>
		{
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

			Pool.Instance.Factory = new PoolFactory(conStr.GetConnectionString(true), PoolCount);
		}

		public override void Execute(string Query, params object[] Parameters)
		{
			while (true)
			{
				try
				{
					ConnectionHolder con = Pool.Instance.Pull();

					MySqlCommand command = CreateCommand(con, Query, Parameters);
					command.ExecuteNonQuery();
					command.Dispose();

					Pool.Instance.Push(con);

					break;
				}
				catch
				{
				}
			}
		}

		public override DataTable ExecuteWithReturnDataTable(string Query, params object[] Parameters)
		{
			while (true)
			{
				try
				{
					ConnectionHolder con = Pool.Instance.Pull();

					MySqlCommand command = CreateCommand(con, Query, Parameters);
					MySqlDataAdapter adapter = new MySqlDataAdapter(command);

					DataTable table = new DataTable();
					adapter.Fill(table);

					adapter.Dispose();
					command.Dispose();
					command.Cancel();

					Pool.Instance.Push(con);

					return table;
				}
				catch
				{
				}
			}
		}

		private MySqlCommand CreateCommand(ConnectionHolder Connection, string Query, object[] Parameters)
		{
			Debug.Assert(Parameters == null || Parameters.Length % 2 == 0, "Parameters count must be even");

			MySqlCommand command = new MySqlCommand(Query, Connection.Connection);
			command.CommandType = CommandType.Text;

			if (Parameters != null)
				for (int i = 0; i < Parameters.Length; i += 2)
					command.Parameters.Add(new MySqlParameter(Parameters[i].ToString(), Parameters[i + 1]));

			return command;
		}
	}
}