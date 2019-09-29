// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.SyntaxTree
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
