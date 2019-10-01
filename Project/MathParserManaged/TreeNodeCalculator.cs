// Copyright 2019. All Rights Reserved.
using GameFramework.MathParser.SyntaxTree;

namespace GameFramework.MathParser
{
	public static class TreeNodeCalculator
	{
		public static double Calculate(TreeNode Node, ArgumentsMap Arguments)
		{
			if (Node is NumberNode)
			{
				return ((NumberNode)Node).Number;
			}
			else if (Node is ParameterNode)
			{
				string parameterName = ((ParameterNode)Node).Name;

				if (!Arguments.ContainsKey(parameterName))
					throw new System.Exception("Parameter [" + parameterName + "] doesn't supplied");

				return Arguments[parameterName];
			}
			else if (Node is FunctionNode)
			{
				FunctionNode node = (FunctionNode)Node;

				return PredefinedFunctions.Calculate(node.Name, node.Parameters, Arguments);
			}
			else if (Node is LogicalOperatorNode)
			{
				LogicalOperatorNode node = (LogicalOperatorNode)Node;

				double leftValue = Calculate(node.LeftNode, Arguments);
				double rightValue = Calculate(node.RightNode, Arguments);

				switch (node.Operator)
				{
					case LogicalOperatorNode.Operators.Equal:
						return (leftValue == rightValue ? 1 : 0);

					case LogicalOperatorNode.Operators.Greater:
						return (leftValue > rightValue ? 1 : 0);

					case LogicalOperatorNode.Operators.Less:
						return (leftValue < rightValue ? 1 : 0);

					case LogicalOperatorNode.Operators.GreaterEqual:
						return (leftValue >= rightValue ? 1 : 0);

					case LogicalOperatorNode.Operators.LessEqual:
						return (leftValue <= rightValue ? 1 : 0);

					default:
						throw new System.Exception("Unknown Logical Operator type");
				}
			}
			else if (Node is ArithmeticOperatorNode)
			{
				ArithmeticOperatorNode node = (ArithmeticOperatorNode)Node;

				double leftValue = Calculate(node.LeftNode, Arguments);
				double rightValue = Calculate(node.RightNode, Arguments);

				switch (node.Operator)
				{
					case ArithmeticOperatorNode.Operators.Addition:
						return leftValue + rightValue;

					case ArithmeticOperatorNode.Operators.Subtraction:
						return leftValue - rightValue;

					case ArithmeticOperatorNode.Operators.Multiplication:
						return leftValue * rightValue;

					case ArithmeticOperatorNode.Operators.Division:
						return leftValue / rightValue;

					case ArithmeticOperatorNode.Operators.Remainder:
						return leftValue % rightValue;

					case ArithmeticOperatorNode.Operators.Power:
						return System.Math.Pow(leftValue, rightValue);

					default:
						throw new System.Exception("Unknown Arithmetic Operator type");
				}
			}

			throw new System.Exception("Unknown TreeNode type");
		}
	}
}