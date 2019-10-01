// Copyright 2019. All Rights Reserved.

namespace GameFramework.MathParser.SyntaxTree
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
