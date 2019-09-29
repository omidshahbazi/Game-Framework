// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.IO;

namespace Zorvan.Framework.Common.FileLayer
{
	public class LogManager
	{
		private string path;
		private string fileName;
		private string oldFileNamePattern;
		private StreamWriter writer;

		private LogManager(string Path, string FileName)
		{
			path = Path;
			fileName = FileName;
			oldFileNamePattern = fileName + " {0}";

			if (!FileSystem.DirectoryExists(path))
				FileSystem.CreateDirectory(path);

			if (FileSystem.FileExists(fileName))
				try
				{
					FileSystem.RenameFile(fileName, string.Format(oldFileNamePattern, DateTime.Now.ToString().Replace(':', '-')));
				}
				catch
				{ }

			writer = FileSystem.CreateStreamWriter(fileName);
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
