// Copyright 2019. All Rights Reserved.

namespace GameFramework.MathParser.SyntaxTree
{
	abstract class OperatorNode : TreeNode
	{
		public TreeNode LeftNode
		{
			get;
			set;
		}

		public TreeNode RightNode
		{
			get;
			set;
		}
	}
}
