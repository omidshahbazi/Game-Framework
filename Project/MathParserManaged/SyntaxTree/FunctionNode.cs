// Copyright 2019. All Rights Reserved.
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
