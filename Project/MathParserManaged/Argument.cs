// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

using System.Collections.Generic;

namespace Zorvan.Framework.MathParser
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