// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class Table
	{
		public string Name
		{
			get;
			private set;
		}

		private List<Column> columns;

		public Column[] Columns
		{
			get { return columns.ToArray(); }
		}

		public Index Index
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

		public Table(string name, Engines Engine = Engines.InnoDB)
		{
			this.Name = name;
			this.columns = new List<Column>();
			this.Collate = Collates.Server_Defualt;
			this.Engine = Engine;
		}

		public Table(string name, Collates collate, Engines Engine = Engines.InnoDB) : this(name, Engine)
		{
			this.Collate = collate;
		}

		public Table(string name, params Column[] columns) : this(name)
		{
			this.columns.AddRange(columns);
		}

		public Table(string name, Engines Engine, params Column[] columns) : this(name, Engine)
		{
			this.columns.AddRange(columns);
		}

		public Table(string name, Index index, params Column[] columns) : this(name)
		{
			this.columns.AddRange(columns);
			this.Index = index;
		}

		public Table(string name, Engines Engine, Index index, params Column[] columns) : this(name, Engine)
		{
			this.columns.AddRange(columns);
			this.Index = index;
		}

		public Table(string name, Collates collate, params Column[] columns) : this(name, collate)
		{
			this.columns.AddRange(columns);
		}

		public Table(string name, Collates collate, Engines Engine, params Column[] columns) : this(name, collate, Engine)
		{
			this.columns.AddRange(columns);
		}

		public void AddColumn(Column col)
		{
			columns.Add(col);
		}
	}
}

