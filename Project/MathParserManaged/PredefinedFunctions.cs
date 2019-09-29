// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.Collections.Generic;
using Zorvan.Framework.MathParser.SyntaxTree;

namespace Zorvan.Framework.MathParser
{
	public static class PredefinedFunctions
	{
		private static Dictionary<string, FunctionDefinition> functions = null;

		private static Random rnd = new Random();

		public static FunctionDefinition[] Functions
		{
			get
			{
				FunctionDefinition[] funcs = new FunctionDefinition[functions.Count];
				functions.Values.CopyTo(funcs, 0);
				return funcs;
			}
		}

		static PredefinedFunctions()
		{
			functions = new Dictionary<string, FunctionDefinition>();

			Register(new FunctionDefinition("if", "if (condition, then expression, else expression)", Function_if));
			Register(new FunctionDefinition("log", "log(x)", Function_log));
			Register(new FunctionDefinition("abs", "abs(x)", Function_abs));
			Register(new FunctionDefinition("acos", "acos(x)", Function_acos));
			Register(new FunctionDefinition("asin", "asin(x)", Function_asin));
			Register(new FunctionDefinition("atan", "atan(x)", Function_atan));
			Register(new FunctionDefinition("cos", "cos(x)", Function_cos));
			Register(new FunctionDefinition("cosh", "cosh(x)", Function_cosh));
			Register(new FunctionDefinition("sin", "sin(x)", Function_sin));
			Register(new FunctionDefinition("sinh", "sinh(x)", Function_sinh));
			Register(new FunctionDefinition("tan", "tan(x)", Function_tan));
			Register(new FunctionDefinition("tanh", "tanh(x)", Function_tanh));
			Register(new FunctionDefinition("ceiling", "ceiling(x)", Function_ceiling));
			Register(new FunctionDefinition("exp", "exp(x)", Function_exp));
			Register(new FunctionDefinition("floor", "floor(x)", Function_floor));
			Register(new FunctionDefinition("round", "round(x)", Function_Round));
			Register(new FunctionDefinition("min", "min(x, y", Function_min));
			Register(new FunctionDefinition("max", "max(x, y)", Function_max));
			Register(new FunctionDefinition("random", "random(minimum, maximum)", Function_random));
		}

		public static void Register(FunctionDefinition Function)
		{
			functions[Function.Name] = Function;
		}

		public static double Calculate(string Name, TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			if (!functions.ContainsKey(Name))
				throw new Exception("Function [" + Name + "] not found");

			return functions[Name].Calculate(Arguments, ArgsMap);
		}

		private static void CheckArguments(TreeNodeCollection Arguments, string Name, int DesiredCount)
		{
			if (Arguments.Count == DesiredCount)
				return;

			throw new Exception("Arguments counts in [" + Name + "] should be [" + DesiredCount + "], but there are [" + Arguments.Count + "]");
		}

		private static double Function_if(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "if", 3);

			double condition = TreeNodeCalculator.Calculate(Arguments[0], ArgsMap);
			if (condition == 1)
				return TreeNodeCalculator.Calculate(Arguments[1], ArgsMap);

			return TreeNodeCalculator.Calculate(Arguments[2], ArgsMap);
		}

		private static double Function_log(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "log", 1);

			return Math.Log10(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_abs(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "abs", 1);

			return Math.Abs(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_acos(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "acos", 1);

			return Math.Acos(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_asin(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "asin", 1);

			return Math.Asin(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_atan(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "atan", 1);

			return Math.Atan(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_cos(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "cos", 1);

			return Math.Cos(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_cosh(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "cosh", 1);

			return Math.Cosh(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_sin(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "sin", 1);

			return Math.Sin(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_sinh(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "sinh", 1);

			return Math.Sinh(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_tan(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "tan", 1);

			return Math.Tan(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_tanh(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "tanh", 1);

			return Math.Tanh(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_ceiling(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "ceiling", 1);

			return Math.Ceiling(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_exp(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "exp", 1);

			return Math.Exp(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_floor(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "floor", 1);

			return Math.Floor(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_Round(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "round", 1);

			return Math.Round(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap));
		}

		private static double Function_max(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "max", 2);

			return Math.Max(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap), TreeNodeCalculator.Calculate(Arguments[1], ArgsMap));
		}

		private static double Function_min(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "min", 2);

			return Math.Min(TreeNodeCalculator.Calculate(Arguments[0], ArgsMap), TreeNodeCalculator.Calculate(Arguments[1], ArgsMap));
		}

		private static double Function_random(TreeNodeCollection Arguments, ArgumentsMap ArgsMap)
		{
			CheckArguments(Arguments, "random", 2);

			return (TreeNodeCalculator.Calculate(Arguments[0], ArgsMap) + rnd.NextDouble() * (TreeNodeCalculator.Calculate(Arguments[1], ArgsMap) - TreeNodeCalculator.Calculate(Arguments[0], ArgsMap)));
		}
	}
}