// Copyright 2019. All Rights Reserved.
using System.IO;

namespace GameFramework.Common.FileLayer
{
	public static class FileSystem
	{
		public static string DataPath
		{
			get;
			set;
		}

		public static string Read(string Path)
		{
			if (!FileExists(Path))
				return string.Empty;

			return File.ReadAllText(DataPath + Path);
		}

		public static byte[] ReadBytes(string Path)
		{
			if (!FileExists(Path))
				return null;

			return File.ReadAllBytes(DataPath + Path);
		}

		public static void Write(string Path, string Contents)
		{
			if (!DirectoryExists(Path))
				CreateDirectory(Path);

			File.WriteAllText(DataPath + Path, Contents);
		}

		public static void Write(string Path, byte[] Contents)
		{
			if (!DirectoryExists(Path))
				CreateDirectory(Path);

			File.WriteAllBytes(DataPath + Path, Contents);
		}

		public static StreamWriter CreateStreamWriter(string Path)
		{
			return new StreamWriter(DataPath + Path);
		}

		public static string GetFileName(string Path)
		{
			return System.IO.Path.GetFileName(DataPath + Path);
		}

		public static string GetFileNameWithoutExtension(string Path)
		{
			return System.IO.Path.GetFileNameWithoutExtension(DataPath + Path);
		}

		public static string GetFileExtension(string Path)
		{
			return System.IO.Path.GetExtension(Path);
		}

		public static bool FileExists(string Path)
		{
			return File.Exists(DataPath + Path);
		}

		public static void DeleteFile(string Path)
		{
			File.Delete(DataPath + Path);
		}

		public static string GetDirectoryName(string Path)
		{
			return System.IO.Path.GetDirectoryName(Path);
		}

		public static bool DirectoryExists(string Path)
		{
			return Directory.Exists(System.IO.Path.GetDirectoryName(DataPath + Path));
		}

		public static void CreateDirectory(string Path)
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DataPath + Path));
		}

		public static string[] GetFiles(string Path)
		{
			return Directory.GetFiles(DataPath + Path);
		}

		public static string[] GetFiles(string Path, string SearchPattern, SearchOption SearchOption = SearchOption.AllDirectories)
		{
			if (!DirectoryExists(Path))
				return null;

			return Directory.GetFiles(DataPath + Path, SearchPattern, SearchOption);
		}

		public static void RenameFile(string OldFilePath, string NewFileName)
		{
			string oldFileDir = Path.GetDirectoryName(OldFilePath);

			File.Move(DataPath + OldFilePath, DataPath + (string.IsNullOrEmpty(oldFileDir) ? "" : "/") + NewFileName);
		}

		public static void CopyAllFiles(string From, string To, bool Overwrite = false)
		{
			if (!Directory.Exists(To))
				Directory.CreateDirectory(To);

			string[] items = Directory.GetFiles(From);

			for (int i = 0; i < items.Length; i++)
			{
				string destPath = Path.Combine(To, Path.GetFileName(items[i]));

				File.Copy(items[i], destPath, Overwrite);
			}

			items = Directory.GetDirectories(From);

			for (int i = 0; i < items.Length; i++)
			{
				string item = items[i];
				string destDir = To + item.Substring(item.LastIndexOf('/'));
				CopyAllFiles(items[i], Path.Combine(To, destDir), Overwrite);
			}
		}

		public static void DeleteAll(string Path)
		{
			Directory.Delete(Path, true);
			Directory.CreateDirectory(Path);
		}
	}
}