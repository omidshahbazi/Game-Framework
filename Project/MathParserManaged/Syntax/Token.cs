// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.

namespace Zorvan.Framework.MathParser.Syntax
{
	class Token
	{
		public enum Types
		{
			Number = 0,
			Parameter,

			Function,
			Equal,
			Greater,
			Less,
			GreaterEqual,
			LessEqual,

			Comma,

			OpenParentheses,
			CloseParentheses,

			Addition,
			Subtraction,
			Multiplication,
			Division,
			Remainder,
			Power,

			EndOfFile,
			Unknown
		}

		public Types Type
		{
			get;
			private set;
		}

		public int LineNumber
		{
			get;
			private set;
		}

		public int ColumnNumber
		{
			get;
			private set;
		}

		public string Value
		{
			get;
			private set;
		}

		public Token(Types Type, int LineNumber, int ColumnNumber, string Value)
		{
			this.Type = Type;
			this.LineNumber = LineNumber;
			this.ColumnNumber = ColumnNumber;
			this.Value = Value;
		}

		public override string ToString()
		{
			return "(" + LineNumber + ", " + ColumnNumber + ") " + Type + " " + Value;
		}
	}
}
