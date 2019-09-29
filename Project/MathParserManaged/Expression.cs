// Copyright 2019. All Rights Reserved.
using System.Text;
using Zorvan.Framework.MathParser.Syntax;
using Zorvan.Framework.MathParser.SyntaxTree;

namespace Zorvan.Framework.MathParser
{
	public class Expression
	{
		private TreeNode node = null;

		public Expression(string Expr)
		{
			node = new Lexer(Expr).Parse();
		}

		public double Calculate(params Argument[] Arguments)
		{
			ArgumentsMap arguments = new ArgumentsMap();
			if (Arguments != null)
				for (int i = 0; i < Arguments.Length; ++i)
				{
					Argument arg = Arguments[i];
					arguments[arg.Name] = arg.Value;
				}

			return TreeNodeCalculator.Calculate(node, arguments);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			Print(node, 0, builder);

			return builder.ToString();
		}

		private static void Print(TreeNode Node, int Indent, StringBuilder Builder)
		{
			++Indent;

			if (Node is NumberNode)
			{
				PrintLine(((NumberNode)Node).Number.ToString(), Indent, Builder);
			}
			else if (Node is ParameterNode)
			{
				PrintLine(((ParameterNode)Node).Name, Indent, Builder);
			}
			else if (Node is FunctionNode)
			{
				FunctionNode node = (FunctionNode)Node;

				PrintLine(node.Name + " (", Indent, Builder);
				TreeNodeCollection nodes = node.Parameters;
				for (int i = 0; i < nodes.Count; ++i)
					Print(nodes[i], Indent + 1, Builder);
				PrintLine(" )", Indent, Builder);
			}
			else if (Node is LogicalOperatorNode)
			{
				LogicalOperatorNode node = (LogicalOperatorNode)Node;

				Print(node.LeftNode, Indent, Builder);
				PrintLine(node.Operator.ToString(), Indent, Builder);
				Print(node.RightNode, Indent, Builder);
			}
			else if (Node is ArithmeticOperatorNode)
			{
				ArithmeticOperatorNode node = (ArithmeticOperatorNode)Node;

				Print(node.LeftNode, Indent, Builder);
				PrintLine(node.Operator.ToString(), Indent, Builder);
				Print(node.RightNode, Indent, Builder);
			}
			else
				throw new System.Exception("Unknown TreeNode type");
		}

		private static void PrintLine(string Text, int Indent, StringBuilder Builder)
		{
			for (int i = 0; i < Indent; ++i)
				Builder.Append(' ');

			Builder.AppendLine(Text);
		}
	}
}