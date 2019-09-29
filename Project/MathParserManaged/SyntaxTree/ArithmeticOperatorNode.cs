// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.SyntaxTree
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
