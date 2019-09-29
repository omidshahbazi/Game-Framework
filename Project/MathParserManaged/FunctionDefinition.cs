// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using Zorvan.Framework.MathParser.SyntaxTree;

namespace Zorvan.Framework.MathParser
{
	public class FunctionDefinition
	{
		private Func<TreeNodeCollection, ArgumentsMap, double> function = null;

		public string Name
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public FunctionDefinition(string Name, string Description, Func<TreeNodeCollection, ArgumentsMap, double> Function)
		{
			this.Name = Name;
			this.Description = Description;
			function = Function;
		}

		public double Calculate(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			return function(Arguments, ArgsMap);
		}
	}
}