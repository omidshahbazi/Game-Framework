// Copyright 2019. All Rights Reserved.

namespace GameFramework.MathParser.SyntaxTree
{
	class ArithmeticOperatorNode : OperatorNode
	{
		public enum Operators
		{
			Addition = 0,
			Subtraction,
			Multiplication,
			Division,
			Remainder,
			Power,
			Unknown
		}

		public Operators Operator
		{
			get;
			private set;
		}

		public ArithmeticOperatorNode(Operators Operator)
		{
			this.Operator = Operator;
		}
	}
}
