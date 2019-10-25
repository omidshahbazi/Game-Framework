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
			string filePath = Path + (Path.EndsWith("\\") || Path.EndsWith("/") ? "" : "/") + FileName;

			if (!FileSystem.DirectoryExists(filePath))
				FileSystem.CreateDirectory(filePath);

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

		public void Log(string Message)
		{
			writer.Write("[Log] ");
			writer.WriteLine(Message);
		}

		public void LogError(string Message)
		{
			writer.Write("[Error] ");
			writer.WriteLine(Message);
		}

		public void LogWarning(string Message)
		{
			writer.Write("[Warning] ");
			writer.WriteLine(Message);
		}
	}
}
