// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;


namespace GameFramework.DatabaseManaged.Generator
{
	public class Database
	{
		public string Name
		{
			get;
			private set;
		}

		private List<Table> tables;

		public Table[] Tables
		{
			get { return tables.ToArray(); }
		}

		public Database(string name)
		{
			this.Name = name;
			this.tables = new List<Table>();
		}

		public Database(string name, List<Table> tables) : this(name)
		{
			this.tables = tables;
		}

		public void AddTable(Table table)
		{
			tables.Add(table);
		}

		public Table GetTable(string name)
		{
			int index = -1;

			for (int i = 0; i < tables.Count; ++i)
			{
				if (Tables[i].Name.ToLower() == name.ToLower())
					index = i;
			}
			if (index == -1)
				return null;
			else
				return Tables[index];
		}
	}
}