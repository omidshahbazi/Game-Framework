// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class Catalog
	{
		private List<Table> tables = null;

		public string Name
		{
			get;
			private set;
		}

		public Table[] Tables
		{
			get { return tables.ToArray(); }
		}

		public Catalog(string Name)
		{
			this.Name = Name;
			tables = new List<Table>();
		}

		public Catalog(string Name, params Table[] Tables) : this(Name)
		{
			tables.AddRange(Tables);
		}

		public Catalog(string Name, List<Table> Tables) : this(Name)
		{
			tables = Tables;
		}

		public void AddTable(Table Table)
		{
			tables.Add(Table);
		}

		public Table GetTable(string Name)
		{
			for (int i = 0; i < tables.Count; ++i)
			{
				if (Tables[i].Name.ToLower() != Name.ToLower())
					continue;

				return Tables[i];
			}

			return null;
		}
	}
}