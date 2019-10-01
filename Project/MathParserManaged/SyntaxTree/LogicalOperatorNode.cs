// Copyright 2019. All Rights Reserved.

namespace GameFramework.MathParser.SyntaxTree
{
	class LogicalOperatorNode : OperatorNode
	{
		public enum Operators
		{
			Equal,
			Greater,
			Less,
			GreaterEqual,
			LessEqual,
			Unknown
		}

		public Operators Operator
		{
			get;
			private set;
		}

		public LogicalOperatorNode(Operators Operator)
		{
			this.Operator = Operator;
		}
	}
}
