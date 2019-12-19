// Copyright 2019. All Rights Reserved.
using System;
using System.IO;

namespace GameFramework.Common.FileLayer
{
	public class LogManager
	{
		private StreamWriter writer;

		public LogManager(string Path, string FileName)
		{
			string filePath = Path + (Path.EndsWith("\\") || Path.EndsWith("/") ? "" : "/");

			if (!FileSystem.DirectoryExists(filePath))
				FileSystem.CreateDirectory(filePath);

			filePath += FileName;

			if (FileSystem.FileExists(filePath))
				try
				{
					FileSystem.RenameFile(filePath, FileSystem.GetDirectoryName(filePath) + "\\" + FileSystem.GetFileNameWithoutExtension(filePath) + " " + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + FileSystem.GetFileExtension(filePath));
				}
				catch
				{ }

			writer = FileSystem.CreateStreamWriter(filePath);
			writer.AutoFlush = true;
		}

		~LogManager()
		{
			writer.Close();
		}

		public void Log(string Message, params object[] Arguments)
		{
			writer.Write("[Log] ");
			writer.WriteLine(string.Format(Message, Arguments));
		}

		public void LogWarning(string Message, params object[] Arguments)
		{
			writer.Write("[Warning] ");
			writer.WriteLine(string.Format(Message, Arguments));
		}

		public void LogError(string Message, params object[] Arguments)
		{
			writer.Write("[Error] ");
			writer.WriteLine(string.Format(Message, Arguments));
		}

		public void LogException(string Message, params object[] Arguments)
		{
			writer.Write("[Exception] ");
			writer.WriteLine(string.Format(Message, Arguments));
		}
	}
}
