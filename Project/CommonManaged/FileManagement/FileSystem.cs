// Copyright 2019. All Rights Reserved.
using System.IO;

namespace GameFramework.Common.FileLayer
{
	public static class FileSystem
	{
		private static string dataPath;

		public static string DataPath
		{
			get { return dataPath; }
			set
			{
				if (!value.EndsWith("/") && !value.EndsWith("\\"))
					value += "/";

				dataPath = value.Replace('\\', '/');
			}
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
			string fullPath = DataPath + Path;

			if (System.IO.Path.HasExtension(fullPath))
				return Directory.Exists(System.IO.Path.GetDirectoryName(fullPath));

			return Directory.Exists(fullPath);
		}

		public static void CreateDirectory(string Path)
		{
			string fullPath = DataPath + Path;

			if (System.IO.Path.HasExtension(fullPath))
			{
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));

				return;
			}

			Directory.CreateDirectory(fullPath);
		}

		public static string[] GetFiles(string Path)
		{
			return GetFiles(Path, "*.*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFiles(string Path, string SearchPattern, SearchOption SearchOption = SearchOption.AllDirectories)
		{
			if (!DirectoryExists(Path))
				return null;

			string[] files = Directory.GetFiles(DataPath + Path, SearchPattern, SearchOption);

			for (int i = 0; i < files.Length; ++i)
				files[i] = files[i].Substring(DataPath.Length);

			return files;
		}

		public static void RenameFile(string OldFilePath, string NewFileName)
		{
			string oldFileDir = Path.GetDirectoryName(OldFilePath);

			File.Move(DataPath + OldFilePath, DataPath + (string.IsNullOrEmpty(oldFileDir) ? "" : "/") + NewFileName);
		}

		public static void CopyAllFiles(string From, string To, bool Overwrite = false)
		{
			string from = DataPath + From;
			string to = DataPath + To;

			if (!Directory.Exists(to))
				Directory.CreateDirectory(to);

			string[] items = Directory.GetFiles(from);

			for (int i = 0; i < items.Length; i++)
			{
				string destPath = Path.Combine(to, Path.GetFileName(items[i]));

				File.Copy(items[i], destPath, Overwrite);
			}

			items = Directory.GetDirectories(from);

			for (int i = 0; i < items.Length; i++)
			{
				string item = items[i];
				string destDir = to + item.Substring(item.LastIndexOf('/'));
				CopyAllFiles(items[i], Path.Combine(to, destDir), Overwrite);
			}
		}

		public static void DeleteAll(string Path)
		{
			Directory.Delete(Path, true);
			Directory.CreateDirectory(Path);
		}
	}
}