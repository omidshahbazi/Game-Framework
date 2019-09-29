// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;

namespace Zorvan.Framework.MathParser.SyntaxTree
{
	class FunctionNode : TreeNode
	{
		public string Name
		{
			get;
			set;
		}

		public TreeNodeCollection Parameters
		{
			get;
			set;
		}

		public FunctionNode(string Name)
		{
			Parameters = new TreeNodeCollection();
			this.Name = Name;
		}
	}
}
