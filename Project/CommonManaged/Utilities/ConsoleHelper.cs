// Copyright 2019. All Rights Reserved.
using System;
using System.IO;
using System.Reflection;

namespace GameFramework.Common.Utilities
{
	public static class ConsoleHelper
	{
		public static string ExecutingPath
		{
			get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\"; }
		}

		public static string ExecutableFileName
		{
			get { return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location); }
		}

		public static void ReadConnectionInfo(out string ServerAddress, out string DatabaseName, out string Username, out string Password)
		{
			ReadString("Enter server address :", out ServerAddress);
			ReadString("Enter database name :", out DatabaseName);
			ReadString("Enter username :", out Username);
			ReadString("Enter password :", out Password);
		}

		public static void ReadBoolean(string Message, out bool Value)
		{
			string value;
			ReadString(Message, out value);

			Value = (value.Length == 1 && value[0] == 'y');
		}

		public static void ReadString(string Message, out string Value)
		{
			Console.WriteLine(Message);

			while (string.IsNullOrEmpty(Value = Console.ReadLine())) ;
		}

		public static void ReadInteger(string Message, out int Value)
		{
			string value;
			ReadString(Message, out value);

			Value = Convert.ToInt32(value);
		}

		public static void ReadFloat(string Message, out float Value)
		{
			string value;
			ReadString(Message, out value);

			Value = Convert.ToSingle(value);
		}

		public static bool GetConfirmation(string Text = "")
		{
			if (string.IsNullOrEmpty(Text))
				Text = "Would you like to continue ? (y/n)";

			bool result = false;
			ReadBoolean(Text, out result);

			return result;
		}

		public static void WriteInfo(string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Info");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			Console.ResetColor();
		}

		public static void WriteWarning(string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.DarkYellow;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Warning");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			Console.ResetColor();
		}

		public static void WriteError(string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.DarkRed;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Error");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			Console.ResetColor();
		}

		public static void WriteDebug(string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.Cyan;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Debug");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			Console.ResetColor();
		}

		public static void WriteCritical(string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Critical");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			Console.ResetColor();
		}

		public static void WriteException(Exception E, string Format, params object[] Args)
		{
			ConsoleColor defaultBackColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("Exception");
			Console.BackgroundColor = defaultBackColor;

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" ");

			Console.WriteLine(Format, Args);

			if (E != null)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine(E.ToString());
				Console.WriteLine(E.StackTrace);
			}

			Console.ResetColor();
		}
	}
}