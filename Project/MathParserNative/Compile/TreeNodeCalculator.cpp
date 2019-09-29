// Copyright 2019. All Rights Reserved.
#include "..\Include\TreeNodeCalculator.h"
#include "..\Include\SyntaxTree\ArithmeticOperatorNode.h"
#include "..\Include\SyntaxTree\FunctionNode.h"
#include "..\Include\SyntaxTree\LogicalOperatorNode.h"
#include "..\Include\SyntaxTree\NumberNode.h"
#include "..\Include\SyntaxTree\ParameterNode.h"
#include "..\Include\PredefinedFunctions.h"
#include <sstream>

namespace Zorvan::Framework::MathParser
{
	double TreeNodeCalculator::Calculate(TreeNode *Node, ArgumentsMap Arguments)
	{
		PredefinedFunctions::Initialize();

		if (IS_CHILD_OF(Node, NumberNode))
		{
			return reinterpret_cast<NumberNode*>(Node)->GetNumber();
		}
		else if (IS_CHILD_OF(Node, ParameterNode))
		{
			std::string parameterName = reinterpret_cast<ParameterNode*>(Node)->GetName();

			if (Arguments.find(parameterName) == Arguments.end())
			{
				std::stringstream ss;
				ss << "Parameter [" << parameterName << "] doesn't supplied";
				throw std::exception(ss.str().c_str());
			}

			return Arguments[parameterName];
		}
		else if (IS_CHILD_OF(Node, FunctionNode))
		{
			FunctionNode *node = reinterpret_cast<FunctionNode*>(Node);

			return PredefinedFunctions::Calculate(node->GetName(), node->GetParameters(), Arguments);
		}
		else if (IS_CHILD_OF(Node, LogicalOperatorNode))
		{
			LogicalOperatorNode *node = reinterpret_cast<LogicalOperatorNode*>(Node);

			double leftValue = Calculate(node->GetLeftNode(), Arguments);
			double rightValue = Calculate(node->GetRightNode(), Arguments);

			switch (node->GetOperator())
			{
			case LogicalOperatorNode::Operators::Equal:
				return (leftValue == rightValue ? 1 : 0);

			case LogicalOperatorNode::Operators::Greater:
				return (leftValue > rightValue ? 1 : 0);

			case LogicalOperatorNode::Operators::Less:
				return (leftValue < rightValue ? 1 : 0);

			case LogicalOperatorNode::Operators::GreaterEqual:
				return (leftValue >= rightValue ? 1 : 0);

			case LogicalOperatorNode::Operators::LessEqual:
				return (leftValue <= rightValue ? 1 : 0);

			default:
				throw std::exception("Unknown Logical Operator type");
			}
		}
		else if (IS_CHILD_OF(Node, ArithmeticOperatorNode))
		{
			ArithmeticOperatorNode *node = reinterpret_cast<ArithmeticOperatorNode*>(Node);

			double leftValue = Calculate(node->GetLeftNode(), Arguments);
			double rightValue = Calculate(node->GetRightNode(), Arguments);

			switch (node->GetOperator())
			{
			case ArithmeticOperatorNode::Operators::Addition:
				return leftValue + rightValue;

			case ArithmeticOperatorNode::Operators::Subtraction:
				return leftValue - rightValue;

			case ArithmeticOperatorNode::Operators::Multiplication:
				return leftValue * rightValue;

			case ArithmeticOperatorNode::Operators::Division:
				return leftValue / rightValue;

			case ArithmeticOperatorNode::Operators::Remainder:
				return fmodf(leftValue, rightValue);

			case ArithmeticOperatorNode::Operators::Power:
				return powf(leftValue, rightValue);

			default:
				throw std::exception("Unknown Arithmetic Operator type");
			}
		}

		throw std::exception("Unknown TreeNode type");
	}
}