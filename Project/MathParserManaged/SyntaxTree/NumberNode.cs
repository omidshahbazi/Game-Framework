// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.SyntaxTree
{
	class NumberNode : TreeNode
	{
		public double Number
		{
			get;
			private set;
		}

		public NumberNode(double Number)
		{
			this.Number = Number;
		}
	}
}
