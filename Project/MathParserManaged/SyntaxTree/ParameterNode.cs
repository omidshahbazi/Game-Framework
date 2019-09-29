// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.SyntaxTree
{
	class ParameterNode : TreeNode
	{
		public string Name
		{
			get;
			private set;
		}

		public ParameterNode(string Name)
		{
			this.Name = Name;
		}
	}
}
