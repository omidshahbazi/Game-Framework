// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
#pragma once

#ifndef LEXER_H
#define LEXER_H

#include <string>
#include <list>
#include <stack>

namespace Zorvan::Framework::MathParser::SyntaxTree
{
	class TreeNode;
}

using namespace Zorvan::Framework::MathParser::SyntaxTree;

namespace Zorvan::Framework::MathParser::Syntax
{
	class Token;

	class Lexer
	{
	public:
		Lexer(const std::string &Expr);

		TreeNode *Parse(void);

		std::string ToString(void) const;

	private:
		TreeNode *ParseToken(Token *Token);
		TreeNode *ParseNumber(Token *Token);
		TreeNode *ParseParameter(Token *Token);
		TreeNode *ParseFunction(Token *Token);
		TreeNode *ParseLogicalOperator(void);
		TreeNode *ParseArithmeticOperator(void);
		TreeNode *ParseComma(void);
		TreeNode *ParseOpenParentheses(void);

		inline Token *GetPrevToken(void)
		{
			return m_TokensList[m_TokenIndex - 1];
		}

		inline Token *GetCurrentToken(void)
		{
			if (m_TokenIndex == m_TokenCount)
				return nullptr;

			return m_TokensList[m_TokenIndex];
		}

		inline Token *GetNextToken(void)
		{
			return m_TokensList[m_TokenIndex + 1];
		}

		inline Token *MoveNextToken(void)
		{
			if (m_TokenIndex == m_TokenCount)
				return nullptr;

			++m_TokenIndex;
			return GetCurrentToken();
		}

		inline TreeNode *PeekNode(void)
		{
			if (m_NodeStack.size() == 0)
				return nullptr;

			return m_NodeStack.top();
		}

		inline TreeNode *PopNode(void)
		{
			TreeNode *node = PeekNode();

			if (node != nullptr)
				m_NodeStack.pop();

			return node;
		}

		static int GetTokenPredence(Token *Token);

		static void Print(Token **Tokens, int Count, std::stringstream &Builder);

	private:
		int m_TokenIndex;
		int m_TokenCount;
		Token **m_TokensList;
		std::stack<TreeNode*> m_NodeStack;
	};
}

#endif