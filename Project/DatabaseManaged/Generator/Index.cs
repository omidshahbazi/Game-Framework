// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class Index
	{
		private List<IndexItem> indexItem;
		public IndexItem[] IndexItem
		{
			get { return indexItem.ToArray(); }
		}

		public Index()
		{
			indexItem = new List<IndexItem>();
		}

		public Index(params IndexItem[] items) : this()
		{
			indexItem.AddRange(items);
		}
	}
	public class IndexItem
	{
		public string Name { get; set; }

		private List<string> columns;

		public string[] Columns
		{
			get { return columns.ToArray(); }
		}

		public IndexItem(string indexName, params string[] columnsName)
		{
			this.Name = indexName;
			this.columns = new List<string>();
			this.columns.AddRange(columnsName);
		}
	}
}
