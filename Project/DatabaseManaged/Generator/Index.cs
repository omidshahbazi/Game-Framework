// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class Index
	{
		private List<string> columns = null;

		public string Name { get; set; }

		public string[] Columns
		{
			get { return columns.ToArray(); }
		}

		public Index(string Name, params string[] Columns)
		{
			this.Name = Name;

			columns = new List<string>();

			columns.AddRange(Columns);
		}

		public void AddColumn(string ColumnName)
		{
			columns.Add(ColumnName);
		}

		public void RemmoveColumn(string ColumnName)
		{
			columns.Remove(ColumnName);
		}
	}
}
