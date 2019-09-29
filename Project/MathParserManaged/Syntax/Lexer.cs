// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Zorvan.Framework.MathParser.SyntaxTree;

namespace Zorvan.Framework.MathParser.Syntax
{
	// Shunting Yard
	// https://en.wikipedia.org/wiki/Shunting-yard_algorithm

	// Reverse Polish notation
	// https://en.wikipedia.org/wiki/Reverse_Polish_notation

	class Lexer
	{
		private class CommaNode : TreeNode
		{ }

		private class OpenParenthesesNode : TreeNode
		{ }

		private static Token.Types[] TOKEN_TYPES_PREDENCES = {
			Token.Types.Comma,
			Token.Types.Subtraction,
			Token.Types.Addition,
			Token.Types.Remainder,
			Token.Types.Division,
			Token.Types.Multiplication,
			Token.Types.Power,
			Token.Types.Equal,
			Token.Types.Greater,
			Token.Types.Less,
			Token.Types.GreaterEqual,
			Token.Types.LessEqual
		};

		private int tokenIndex = 0;
		private Token[] tokensList = null;
		private Stack<TreeNode> nodeStack = null;

		public Lexer(string Expr)
		{
			Tokenizer tokenizer = new Tokenizer(Expr);
			Stack<Token> stack = new Stack<Token>();
			List<Token> queue = new List<Token>();

			Token token = null;
			while ((token = tokenizer.ReadNextToken()).Type != Token.Types.EndOfFile)
			{
				switch (token.Type)
				{
					case Token.Types.Number:
					case Token.Types.Parameter:
						{
							queue.Add(token);
						}
						break;

					case Token.Types.Function:
					case Token.Types.OpenParentheses:
						{
							if (stack.Count != 0)
							{
								Token topOfStackToken = stack.Peek();
								if (topOfStackToken.Type == Token.Types.Function)
									queue.Add(token);
							}

							stack.Push(token);
						}
						break;

					case Token.Types.Subtraction:
					case Token.Types.Addition:
					case Token.Types.Remainder:
					case Token.Types.Division:
					case Token.Types.Multiplication:
					case Token.Types.Power:
					case Token.Types.Equal:
					case Token.Types.Greater:
					case Token.Types.Less:
					case Token.Types.GreaterEqual:
					case Token.Types.LessEqual:
					case Token.Types.Comma:
						{
							while (stack.Count != 0)
							{
								Token topOfStackToken = stack.Peek();
								if (topOfStackToken.Type == Token.Types.OpenParentheses)
									break;

								if (topOfStackToken.Type == Token.Types.Function ||
									GetTokenPredence(token) < GetTokenPredence(topOfStackToken))
								{
									topOfStackToken = stack.Pop();
									queue.Add(topOfStackToken);

									continue;
								}

								break;
							}

							if (token.Type == Token.Types.Comma)
								queue.Add(token);
							else
								stack.Push(token);
						}
						break;

					case Token.Types.CloseParentheses:
						{
							Token topOfStackToken = null;

							while (stack.Count != 0)
							{
								topOfStackToken = stack.Peek();
								if (topOfStackToken.Type == Token.Types.OpenParentheses)
									break;

								topOfStackToken = stack.Pop();
								queue.Add(topOfStackToken);
							}

							topOfStackToken = stack.Pop();

							//if (topOfStackToken.Type != Token.Types.OpenParentheses)
							//	throw new Exception("Mismatched parentheses at Ln " + token.LineNumber + ", " + token.ColumnNumber);
						}
						break;

					default:
						throw new Exception("Unknown Token [" + token.Type + " " + token.Value + "] at Ln " + token.LineNumber + ", " + token.ColumnNumber);
				}
			}

			while (stack.Count != 0)
			{
				token = stack.Pop();

				//if (token.Type == Token.Types.OpenParentheses || token.Type == Token.Types.CloseParentheses)
				//	throw new Exception("Mismatched parentheses at Ln " + token.LineNumber + ", " + token.ColumnNumber);

				queue.Add(token);
			}

			tokensList = queue.ToArray();
		}

		public TreeNode Parse()
		{
			nodeStack = new Stack<TreeNode>();

			Token token = GetCurrentToken();
			while (token != null)
			{
				ParseToken(token);

				token = MoveNextToken();
			}

			return PopNode();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			Print(tokensList, builder);

			return builder.ToString();
		}

		private TreeNode ParseToken(Token Token)
		{
			switch (Token.Type)
			{
				case Token.Types.Number:
					return ParseNumber(Token);

				case Token.Types.Parameter:
					return ParseParameter(Token);

				case Token.Types.Function:
					return ParseFunction(Token);

				case Token.Types.Equal:
				case Token.Types.Greater:
				case Token.Types.Less:
				case Token.Types.GreaterEqual:
				case Token.Types.LessEqual:
					return ParseLogicalOperator();

				case Token.Types.Addition:
				case Token.Types.Subtraction:
				case Token.Types.Multiplication:
				case Token.Types.Division:
				case Token.Types.Remainder:
				case Token.Types.Power:
					return ParseArithmeticOperator();

				case Token.Types.Comma:
					return ParseComma(Token);

				case Token.Types.OpenParentheses:
					return ParseOpenParentheses(Token);
			}

			throw new Exception("Unknown Token [" + Token.Type + " " + Token.Value + "] at Ln " + Token.LineNumber + ", " + Token.ColumnNumber);
		}

		private TreeNode ParseNumber(Token Token)
		{
			NumberNode node = new NumberNode(Convert.ToDouble(Token.Value));
			nodeStack.Push(node);
			return node;
		}

		private TreeNode ParseParameter(Token Token)
		{
			ParameterNode node = new ParameterNode(Token.Value);
			nodeStack.Push(node);
			return node;
		}

		private TreeNode ParseFunction(Token Token)
		{
			FunctionNode node = new FunctionNode(Token.Value);

			node.Parameters.Insert(0, PopNode());

			while (true)
			{
				TreeNode parameterNode = PeekNode();

				if (parameterNode is CommaNode)
				{
					PopNode();
					node.Parameters.Insert(0, PopNode());

					continue;
				}

				if (!(parameterNode is OpenParenthesesNode))
					throw new Exception();

				PopNode();

				break;
			}

			nodeStack.Push(node);

			return node;
		}

		private TreeNode ParseLogicalOperator()
		{
			Token token = GetCurrentToken();

			LogicalOperatorNode.Operators op = LogicalOperatorNode.Operators.Unknown;
			switch (token.Type)
			{
				case Token.Types.Equal: op = LogicalOperatorNode.Operators.Equal; break;
				case Token.Types.Greater: op = LogicalOperatorNode.Operators.Greater; break;
				case Token.Types.Less: op = LogicalOperatorNode.Operators.Less; break;
				case Token.Types.GreaterEqual: op = LogicalOperatorNode.Operators.GreaterEqual; break;
				case Token.Types.LessEqual: op = LogicalOperatorNode.Operators.LessEqual; break;
			}

			LogicalOperatorNode node = new LogicalOperatorNode(op);

			node.RightNode = PopNode();
			node.LeftNode = PopNode();

			nodeStack.Push(node);
			return node;
		}

		private TreeNode ParseArithmeticOperator()
		{
			Token token = GetCurrentToken();

			ArithmeticOperatorNode.Operators op = ArithmeticOperatorNode.Operators.Unknown;
			switch (token.Type)
			{
				case Token.Types.Addition: op = ArithmeticOperatorNode.Operators.Addition; break;
				case Token.Types.Subtraction: op = ArithmeticOperatorNode.Operators.Subtraction; break;
				case Token.Types.Multiplication: op = ArithmeticOperatorNode.Operators.Multiplication; break;
				case Token.Types.Division: op = ArithmeticOperatorNode.Operators.Division; break;
				case Token.Types.Remainder: op = ArithmeticOperatorNode.Operators.Remainder; break;
				case Token.Types.Power: op = ArithmeticOperatorNode.Operators.Power; break;
			}

			ArithmeticOperatorNode node = new ArithmeticOperatorNode(op);

			node.RightNode = PopNode();
			node.LeftNode = PopNode();

			nodeStack.Push(node);
			return node;
		}

		private TreeNode ParseComma(Token Token)
		{
			CommaNode node = new CommaNode();
			nodeStack.Push(node);
			return node;
		}

		private TreeNode ParseOpenParentheses(Token Token)
		{
			OpenParenthesesNode node = new OpenParenthesesNode();
			nodeStack.Push(node);
			return node;
		}

		private Token GetCurrentToken()
		{
			if (tokenIndex == tokensList.Length)
				return null;

			return tokensList[tokenIndex];
		}

		private Token MoveNextToken()
		{
			if (tokenIndex == tokensList.Length)
				return null;

			++tokenIndex;
			return GetCurrentToken();
		}

		private TreeNode PeekNode()
		{
			if (nodeStack.Count == 0)
				return null;

			return nodeStack.Peek();
		}

		private TreeNode PopNode()
		{
			if (nodeStack.Count == 0)
				return null;

			return nodeStack.Pop();
		}

		private static int GetTokenPredence(Token Token)
		{
			return Array.IndexOf(TOKEN_TYPES_PREDENCES, Token.Type);
		}

		private static void Print(Token[] Tokens, StringBuilder Builder)
		{
			for (int i = 0; i < Tokens.Length; ++i)
			{
				Token token = Tokens[i];

				Builder.Append(token.Value);

				Builder.Append(' ');
			}
		}
	}
}