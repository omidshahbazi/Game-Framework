// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class Table
	{
		private List<Column> columns;

		public string Name
		{
			get;
			private set;
		}

		public Column[] Columns
		{
			get { return columns.ToArray(); }
		}

		public IndexGroup IndexGroup
		{
			get;
			set;
		}

		public Collates Collate
		{
			get;
			set;
		}

		public Engines Engine
		{
			get;
			set;
		}

		public Table(string Name, Collates Collate = Collates.Defualt, Engines Engine = Engines.InnoDB)
		{
			this.Name = Name;
			this.Collate = Collate;
			this.Engine = Engine;

			columns = new List<Column>();
		}

		public Table(string Name, params Column[] Columns) : this(Name)
		{
			columns.AddRange(Columns);
		}

		public Table(string Name, Engines Engine, params Column[] Columns) : this(Name, Collates.Defualt, Engine)
		{
			columns.AddRange(Columns);
		}

		public Table(string name, IndexGroup IndexGroup, params Column[] Columns) : this(name)
		{
			columns.AddRange(Columns);

			this.IndexGroup = IndexGroup;
		}

		public Table(string name, Engines Engine, IndexGroup IndexGroup, params Column[] Columns) : this(name, Engine)
		{
			columns.AddRange(Columns);

			this.IndexGroup = IndexGroup;
		}

		public Table(string name, Collates Collate, params Column[] Columns) : this(name, Collate)
		{
			columns.AddRange(Columns);
		}

		public Table(string name, Collates Collate, Engines Engine, params Column[] Columns) : this(name, Collate, Engine)
		{
			columns.AddRange(Columns);
		}

		public void AddColumn(Column col)
		{
			columns.Add(col);
		}
	}
}

