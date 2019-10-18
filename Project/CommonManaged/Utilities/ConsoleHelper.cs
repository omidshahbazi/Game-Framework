// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Common.Utilities
{
	public static class ConsoleHelper
	{
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
	}
}