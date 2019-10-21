// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.DatabaseManaged.Generator
{
	public class IndexGroup
	{
		private List<Index> indecies = null;

		public Index[] Indecies
		{
			get { return indecies.ToArray(); }
		}

		public IndexGroup()
		{
			indecies = new List<Index>();
		}

		public IndexGroup(params Index[] Indecies) : this()
		{
			indecies.AddRange(Indecies);
		}
	}
}
