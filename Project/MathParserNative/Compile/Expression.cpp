// Copyright 2019. All Rights Reserved.
#include "..\Include\Expression.h"
#include "..\Include\Syntax\Lexer.h"
#include "..\Include\SyntaxTree\ArithmeticOperatorNode.h"
#include "..\Include\SyntaxTree\FunctionNode.h"
#include "..\Include\SyntaxTree\LogicalOperatorNode.h"
#include "..\Include\SyntaxTree\NumberNode.h"
#include "..\Include\SyntaxTree\ParameterNode.h"

using namespace Zorvan::Framework::MathParser::Syntax;

namespace Zorvan::Framework::MathParser
{
	Expression::Expression(const std::string & Expr)
	{
		m_Node = Lexer(Expr).Parse();
	}

	double Expression::Calculate(const ArgumentCollection &Arguments)
	{
		TreeNodeCalculator::ArgumentsMap arguments;

		for each (auto arg in Arguments)
			arguments[arg->GetName()] = arg->GetValue();

		return TreeNodeCalculator::Calculate(m_Node, arguments);
	}

	std::string Expression::ToString(void) const
	{
		std::stringstream builder;

		Print(m_Node, 0, builder);

		return builder.str();
	}

	void Expression::Print(TreeNode *Node, int Indent, std::stringstream &Builder)
	{
		++Indent;

		if (IS_CHILD_OF(Node, NumberNode))
		{
			std::stringstream ss;
			ss << reinterpret_cast<NumberNode*>(Node)->GetNumber();
			PrintLine(ss.str(), Indent, Builder);
		}
		else if (IS_CHILD_OF(Node, ParameterNode))
		{
			PrintLine(reinterpret_cast<ParameterNode*>(Node)->GetName(), Indent, Builder);
		}
		else if (IS_CHILD_OF(Node, FunctionNode))
		{
			FunctionNode *node = reinterpret_cast<FunctionNode*>(Node);

			PrintLine(node->GetName() + " (", Indent, Builder);

			const TreeNodeCollection &nodes = node->GetParameters();
			for each (const auto &node in nodes)
				Print(node, Indent + 1, Builder);

			PrintLine(" )", Indent, Builder);
		}
		else if (IS_CHILD_OF(Node, LogicalOperatorNode))
		{
			LogicalOperatorNode *node = reinterpret_cast<LogicalOperatorNode*>(Node);

			Print(node->GetLeftNode(), Indent, Builder);
			PrintLine(node->GetOperatorName(), Indent, Builder);
			Print(node->GetRightNode(), Indent, Builder);
		}
		else if (IS_CHILD_OF(Node, ArithmeticOperatorNode))
		{
			ArithmeticOperatorNode *node = reinterpret_cast<ArithmeticOperatorNode*>(Node);

			Print(node->GetLeftNode(), Indent, Builder);
			PrintLine(node->GetOperatorName(), Indent, Builder);
			Print(node->GetRightNode(), Indent, Builder);
		}
		else
			throw std::exception("Unknown TreeNode type");
	}

	void Expression::PrintLine(const std::string &Text, int Indent, std::stringstream &Builder)
	{
		for (int i = 0; i < Indent; ++i)
			Builder << ' ';

		Builder << Text << std::endl;
	}
}