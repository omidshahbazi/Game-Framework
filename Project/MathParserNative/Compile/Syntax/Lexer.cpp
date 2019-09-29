// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#include "..\..\Include\Syntax\Lexer.h"
#include "..\..\Include\Syntax\Token.h"
#include "..\..\Include\Syntax\Tokenizer.h"
#include "..\..\Include\SyntaxTree\ArithmeticOperatorNode.h"
#include "..\..\Include\SyntaxTree\FunctionNode.h"
#include "..\..\Include\SyntaxTree\LogicalOperatorNode.h"
#include "..\..\Include\SyntaxTree\NumberNode.h"
#include "..\..\Include\SyntaxTree\ParameterNode.h"
#include <list>
#include <sstream>

namespace Zorvan::Framework::MathParser::Syntax
{
	// Shunting Yard
	// https://en.wikipedia.org/wiki/Shunting-yard_algorithm

	// Reverse Polish notation
	// https://en.wikipedia.org/wiki/Reverse_Polish_notation

	class CommaNode : public TreeNode
	{ };

	class OpenParenthesesNode : public TreeNode
	{ };

	const Token::Types TOKEN_TYPES_PREDENCES[] = {
		Token::Types::Comma,
		Token::Types::Subtraction,
		Token::Types::Addition,
		Token::Types::Remainder,
		Token::Types::Division,
		Token::Types::Multiplication,
		Token::Types::Power,
		Token::Types::Equal,
		Token::Types::Greater,
		Token::Types::Less,
		Token::Types::GreaterEqual,
		Token::Types::LessEqual
	};

	Lexer::Lexer(const std::string &Expr) :
		m_TokenIndex(0),
		m_TokensList(nullptr)
	{
		Tokenizer tokenizer(Expr);

		std::stack<Token*> stack;
		std::list<Token*> queue;

		Token *token = nullptr;
		while ((token = tokenizer.ReadNextToken())->GetType() != Token::Types::EndOfFile)
		{
			switch (token->GetType())
			{
			case Token::Types::Number:
			case Token::Types::Parameter:
			{
				queue.push_back(token);
			}
			break;

			case Token::Types::Function:
			case Token::Types::OpenParentheses:
			{
				if (stack.size() != 0)
				{
					Token *topOfStackToken = stack.top();
					if (topOfStackToken->GetType() == Token::Types::Function)
						queue.push_back(token);
				}

				stack.push(token);
			}
			break;

			case Token::Types::Subtraction:
			case Token::Types::Addition:
			case Token::Types::Remainder:
			case Token::Types::Division:
			case Token::Types::Multiplication:
			case Token::Types::Power:
			case Token::Types::Equal:
			case Token::Types::Greater:
			case Token::Types::Less:
			case Token::Types::GreaterEqual:
			case Token::Types::LessEqual:
			case Token::Types::Comma:
			{
				while (stack.size() != 0)
				{
					Token *topOfStackToken = stack.top();
					if (topOfStackToken->GetType() == Token::Types::OpenParentheses)
						break;

					if (topOfStackToken->GetType() == Token::Types::Function ||
						GetTokenPredence(token) < GetTokenPredence(topOfStackToken))
					{
						topOfStackToken = stack.top();
						stack.pop();
						queue.push_back(topOfStackToken);

						continue;
					}

					break;
				}

				if (token->GetType() == Token::Types::Comma)
					queue.push_back(token);
				else
					stack.push(token);
			}
			break;

			case Token::Types::CloseParentheses:
			{
				Token *topOfStackToken = nullptr;

				while (stack.size() != 0)
				{
					topOfStackToken = stack.top();
					if (topOfStackToken->GetType() == Token::Types::OpenParentheses)
						break;

					topOfStackToken = stack.top();
					stack.pop();
					queue.push_back(topOfStackToken);
				}

				topOfStackToken = stack.top();
				stack.pop();

				//if (topOfStackToken.Type != Token.Types.OpenParentheses)
				//	throw new Exception("Mismatched parentheses at Ln " + token.LineNumber + ", " + token.ColumnNumber);
			}
			break;

			default:
			{
				std::stringstream ss;
				ss << "Unknown Token [" << token->GetTypeName() << " " << token->GetValue() << "] at Ln " << token->GetLineNumber() << ", " << token->GetColumnNumber();
				throw std::exception(ss.str().c_str());
			}
			}
		}

		while (stack.size() != 0)
		{
			token = stack.top();
			stack.pop();

			//if (token.Type == Token.Types.OpenParentheses || token.Type == Token.Types.CloseParentheses)
			//	throw new Exception("Mismatched parentheses at Ln " + token.LineNumber + ", " + token.ColumnNumber);

			queue.push_back(token);
		}

		m_TokenCount = queue.size();
		m_TokensList = reinterpret_cast<Token**>(malloc(sizeof(Token*) * m_TokenCount));
		int index = 0;
		for each (auto token in queue)
			m_TokensList[index++] = token;
	}

	TreeNode *Lexer::Parse(void)
	{
		m_NodeStack = std::stack<TreeNode*>();

		Token *token = GetCurrentToken();
		while (token != nullptr)
		{
			ParseToken(token);

			token = MoveNextToken();
		}

		return PopNode();
	}

	std::string Lexer::ToString(void) const
	{
		std::stringstream builder;

		Print(m_TokensList, m_TokenCount, builder);

		return builder.str();
	}

	TreeNode *Lexer::ParseToken(Token *Token)
	{
		switch (Token->GetType())
		{
		case Token::Types::Number:
			return ParseNumber(Token);

		case Token::Types::Parameter:
			return ParseParameter(Token);

		case Token::Types::Function:
			return ParseFunction(Token);

		case Token::Types::Equal:
		case Token::Types::Greater:
		case Token::Types::Less:
		case Token::Types::GreaterEqual:
		case Token::Types::LessEqual:
			return ParseLogicalOperator();

		case Token::Types::Addition:
		case Token::Types::Subtraction:
		case Token::Types::Multiplication:
		case Token::Types::Division:
		case Token::Types::Remainder:
		case Token::Types::Power:
			return ParseArithmeticOperator();

		case Token::Types::Comma:
			return ParseComma();

		case Token::Types::OpenParentheses:
			return ParseOpenParentheses();
		}

		std::stringstream ss;
		ss << "Unknown Token [" << Token->GetTypeName() << " " << Token->GetValue() << "] at Ln " << Token->GetLineNumber() << ", " << Token->GetColumnNumber();
		throw std::exception(ss.str().c_str());
	}

	TreeNode *Lexer::ParseNumber(Token *Token)
	{
		NumberNode *node = new NumberNode(atof(Token->GetValue().c_str()));
		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseParameter(Token *Token)
	{
		ParameterNode *node = new ParameterNode(Token->GetValue());
		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseFunction(Token *Token)
	{
		FunctionNode *node = new FunctionNode(Token->GetValue());

		node->InsertParameter(0, PopNode());

		while (true)
		{
			TreeNode *parameterNode = PeekNode();

			if (IS_CHILD_OF(parameterNode, CommaNode))
			{
				PopNode();
				node->InsertParameter(0, PopNode());

				continue;
			}

			if (!IS_CHILD_OF(parameterNode, OpenParenthesesNode))
				throw std::exception();

			PopNode();

			break;
		}

		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseLogicalOperator(void)
	{
		Token *token = GetCurrentToken();

		LogicalOperatorNode::Operators op = LogicalOperatorNode::Operators::Unknown;
		switch (token->GetType())
		{
		case Token::Types::Equal: op = LogicalOperatorNode::Operators::Equal; break;
		case Token::Types::Greater: op = LogicalOperatorNode::Operators::Greater; break;
		case Token::Types::Less: op = LogicalOperatorNode::Operators::Less; break;
		case Token::Types::GreaterEqual: op = LogicalOperatorNode::Operators::GreaterEqual; break;
		case Token::Types::LessEqual: op = LogicalOperatorNode::Operators::LessEqual; break;
		}

		LogicalOperatorNode *node = new LogicalOperatorNode(op);

		node->SetRightNode(PopNode());
		node->SetLeftNode(PopNode());

		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseArithmeticOperator(void)
	{
		Token *token = GetCurrentToken();

		ArithmeticOperatorNode::Operators op = ArithmeticOperatorNode::Operators::Unknown;
		switch (token->GetType())
		{
		case Token::Types::Addition: op = ArithmeticOperatorNode::Operators::Addition; break;
		case Token::Types::Subtraction: op = ArithmeticOperatorNode::Operators::Subtraction; break;
		case Token::Types::Multiplication: op = ArithmeticOperatorNode::Operators::Multiplication; break;
		case Token::Types::Division: op = ArithmeticOperatorNode::Operators::Division; break;
		case Token::Types::Remainder: op = ArithmeticOperatorNode::Operators::Remainder; break;
		case Token::Types::Power: op = ArithmeticOperatorNode::Operators::Power; break;
		}

		ArithmeticOperatorNode *node = new ArithmeticOperatorNode(op);

		node->SetRightNode(PopNode());
		node->SetLeftNode(PopNode());

		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseComma()
	{
		CommaNode *node = new CommaNode();
		m_NodeStack.push(node);
		return node;
	}

	TreeNode *Lexer::ParseOpenParentheses()
	{
		OpenParenthesesNode *node = new OpenParenthesesNode();
		m_NodeStack.push(node);
		return node;
	}

	int Lexer::GetTokenPredence(Token *Token)
	{
		int len = sizeof(TOKEN_TYPES_PREDENCES);
		for (int i = 0; i < len; ++i)
			if (TOKEN_TYPES_PREDENCES[i] == Token->GetType())
				return i;

		return -1;
	}

	void Lexer::Print(Token **Tokens, int Count, std::stringstream &Builder)
	{
		for (int i = 0; i < Count; ++i)
		{
			Token *token = Tokens[i];

			Builder << token->GetValue();

			Builder << ' ';
		}
	}
}