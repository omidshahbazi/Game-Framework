// Copyright 2019. All Rights Reserved.

using System.Collections.Generic;

namespace GameFramework.MathParser
{
	public class Argument
	{
		public string Name
		{
			get;
			private set;
		}

		public double Value
		{
			get;
			private set;
		}

		public Argument(string Name, double Value)
		{
			this.Name = Name;
			this.Value = Value;
		}
	}

	public class ArgumentsMap : Dictionary<string, double>
	{ }
}