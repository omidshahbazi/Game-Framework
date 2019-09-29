// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.SyntaxTree
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
