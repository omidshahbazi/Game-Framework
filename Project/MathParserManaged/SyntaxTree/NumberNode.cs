﻿// Copyright 2019. All Rights Reserved.

namespace GameFramework.MathParser.SyntaxTree
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
